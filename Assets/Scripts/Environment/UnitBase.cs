using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private const string ResourceName = "Resource";

    [SerializeField] private List<UnitSpawnPosition> _unitSpawnPositions;
    [SerializeField] private float _scanRange;

    public event Action<List<Resource>> ResourcesFound;
    public event Action<List<Unit>> UnitsCountChanged;
    public event Action<Dictionary<ResourceType, Counter>> ResourceCountChanged;
    public event Action<ResourceType, UnitBase> NewResourceEntered;
    public event Action<int> BaseWasClicked;

    private readonly List<Resource> _foundResources = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly List<Unit> _units = new();
    private readonly Dictionary<ResourceType, Counter> _resources = new();
    private ResourceSpawner _resourceSpawner;
    private int _number;

    public int Number => _number;

    private void OnDisable()
    {
        foreach (Unit unit in _units)
            unit.WasFreed -= OnWasFreed;
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

    private void OnMouseDown()
    {
        BaseWasClicked?.Invoke(_number);
    }

    public void TakeSpawners(ResourceSpawner resourceSpawner)
    {
        _resourceSpawner = resourceSpawner;
    }

    public void TakeNewDictionary(ResourceType resourceType, Counter counter)
    {
        _resources.Add(resourceType, counter);
    }

    public void TakeNumber(int number)
    {
        _number = number;
    }

    public void AddUnit(Unit unit)
    {
        foreach (UnitSpawnPosition spawnPosition in _unitSpawnPositions)
        {
            if (spawnPosition.IsOccupied == false)
            {
                spawnPosition.Occupy();
                unit.transform.position = spawnPosition.transform.position;
                unit.WasFreed += OnWasFreed;
                unit.TakeSpawnPositin(spawnPosition.transform.position);
                unit.TakeBasePosition(transform.position);
                _units.Add(unit);
                UnitsCountChanged?.Invoke(_units);
                return;
            }
        }
    }

    public bool HasFreeSpace()
    {
        foreach (UnitSpawnPosition spawnPosition in _unitSpawnPositions)
        {
            if (spawnPosition.IsOccupied == false)
            {
                return true;
            }
        }

        return false;
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
        if (_foundResources.Count == 0)
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

    private void AddResource(ResourceType resourceType)
    {
        if (_resources.ContainsKey(resourceType) == false)
            NewResourceEntered?.Invoke(resourceType, this);

        _resources[resourceType].IncreaseCount();
        ResourceCountChanged?.Invoke(_resources);
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

    private void OnWasFreed(Unit unit)
    {
        UnitsCountChanged?.Invoke(_units);
    }
}