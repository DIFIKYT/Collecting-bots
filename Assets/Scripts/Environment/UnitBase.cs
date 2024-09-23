using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private const string ResourceName = "Resource";

    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private List<Transform> _unitSpawnPositions;
    [SerializeField] private float _scanRange;

    public event Action<ResourceType, float> ResourceCountChanged;
    public event Action<List<Resource>> ResourcesFound;
    public event Action<List<Unit>> UnitsCountChanged;

    private float _copperCount;
    private float _ironCount;
    private float _goldCount;
    private readonly int _startUnitCount = 3;
    private readonly List<Resource> _foundResources = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly List<Unit> _units = new();

    private void OnEnable()
    {
        _unitSpawner.UnitSpawned += AddUnit;
    }

    private void OnDisable()
    {
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

    private void AddResource(ResourceType resourceType)
    {
        AddResourceCount(resourceType);

        switch (resourceType)
        {
            case ResourceType.Copper:
                ResourceCountChanged?.Invoke(resourceType, _copperCount);
                break;
            case ResourceType.Iron:
                ResourceCountChanged?.Invoke(resourceType, _ironCount);
                break;
            case ResourceType.Gold:
                ResourceCountChanged?.Invoke(resourceType, _goldCount);
                break;
        }
    }

    private void AddResourceCount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Copper:
                _copperCount++;
                break;
            case ResourceType.Iron:
                _ironCount++;
                break;
            case ResourceType.Gold:
                _goldCount++;
                break;
        }
    }
}