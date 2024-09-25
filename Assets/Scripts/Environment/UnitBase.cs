using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private const string ResourceName = "Resource";

    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private List<Transform> _unitSpawnPositions;
    [SerializeField] private float _scanRange;

    public event Action<Dictionary<ResourceType, Counter>> ResourceCountChanged;
    public event Action<ResourceType, UnitBase> NewResourceEntered;
    public event Action<List<Resource>> ResourcesFound;
    public event Action<List<Unit>> UnitsCountChanged;

    private readonly int _startUnitCount = 3;
    private readonly List<Resource> _foundResources = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly List<Unit> _units = new();
    private readonly Dictionary<ResourceType, Counter> _resources = new();
    private UnitSpawner _unitSpawner;
    private ResourceSpawner _resourceSpawner;

    private void OnEnable()
    {
        if (_unitSpawner != null)
            _unitSpawner.UnitSpawned += AddUnit;
    }

    private void OnDisable()
    {
        if (_unitSpawner != null)
            _unitSpawner.UnitSpawned -= AddUnit;

        foreach (Unit unit in _units)
            unit.WasFreed -= OnWasFreed;
    }


    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            _unitSpawner.Spawn(_unitSpawnPositions[i].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            Resource resource = unit.GetComponentInChildren<Resource>();

            if (resource != null)
            {
                AddResource(resource.Type);
                _resourceSpawner.ReturnToPool(unit.GetResource());
                _selectedResources.Remove(resource);
            }
        }
    }

    public void TakeSpawners(UnitSpawner unitSpawner, ResourceSpawner resourceSpawner)
    {
        _unitSpawner = unitSpawner;
        _resourceSpawner = resourceSpawner;
    }

    public void TakeNewDictionary(ResourceType resourceType, Counter counter)
    {
        _resources.Add(resourceType, counter);
    }
                

    private void AddResource(ResourceType resourceType)
    {
        if (_resources.ContainsKey(resourceType) == false)
            NewResourceEntered?.Invoke(resourceType, this);

        _resources[resourceType].IncreaseCount();
        ResourceCountChanged?.Invoke(_resources);
    }

    public void Scan()
    {
        _foundResources.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRange, LayerMask.GetMask(ResourceName));

        foreach (Collider collider in colliders)
        {
            Resource currentResource = collider.GetComponent<Resource>();

            if (currentResource != null && _selectedResources.Contains(currentResource) == false)
            {
                _foundResources.Add(currentResource);
            }
        }

        ResourcesFound?.Invoke(_foundResources);
    }

    public void SendUnit()
    {
        if (_foundResources.Count <= 0)
            return;

        Unit freeUnit = GetAvailableUnit();
        Resource resource = _foundResources[0];

        if (freeUnit != null)
        {
            freeUnit.MoveTo(resource.transform.position);
            freeUnit.SetResource(resource);
            _foundResources.Remove(resource);
            _selectedResources.Add(resource);
        }

        UnitsCountChanged?.Invoke(_units);
    }

    private Unit GetAvailableUnit()
    {
        foreach (Unit unit in _units)
        {
            if (unit.IsBusy == false)
            {
                return unit;
            }
        }

        return null;
    }

    private void AddUnit(Unit unit)
    {
        _units.Add(unit);
        unit.WasFreed += OnWasFreed;
        UnitsCountChanged?.Invoke(_units);
    }

    private void OnWasFreed(Unit unit)
    {
        UnitsCountChanged?.Invoke(_units);
    }
}