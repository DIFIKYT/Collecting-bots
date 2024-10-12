using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : Spawnable
{
    [SerializeField] private List<UnitSpawnPosition> _unitSpawnPositions;

    private readonly List<Unit> _units = new();
    private readonly Dictionary<ResourceType, Counter> _resources = new();
    private readonly int _copperAmountForBuyUnit = 3;
    private readonly int _ironAmountForBuyBase = 5;
    private Bunner _bunner;
    private Unit _unitBaseCreator;

    public event Action<Dictionary<ResourceType, Counter>> ResourceCountChanged;
    public event Action<Resource> ResourceEntered;
    public event Action<ResourceType, UnitBase> NewResourceEntered;
    public event Action<Vector3, Unit, Bunner> UnitBaseCreated;
    public event Action<Bunner> BunnerMoved;
    public event Action<UnitBase> CopperEnough;

    public List<Unit> Units => _units;
    public int Number { get; private set; }
    public bool CanMoveBunner { get; private set; }
    public bool CanBuyUnit { get; private set; }

    private void Awake()
    {
        CanBuyUnit = true;
        CanMoveBunner = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            Resource resource = unit.Resource;

            if (resource != null && unit.IsResourceTaked && _units.Contains(unit))
            {
                AddResource(resource.Type);
                ResourceEntered?.Invoke(unit.GetResource());
            }
        }
    }

    public void TakeBunner(Bunner bunner)
    {
        if (_bunner != null)
            BunnerMoved?.Invoke(_bunner);

        _bunner = bunner;
        StartCoroutine(WaitUnit());
    }

    public void TakeDictionary(ResourceType resourceType, Counter counter)
    {
        _resources.Add(resourceType, counter);
    }

    public void TakeNumber(int number)
    {
        Number = number;
    }

    public void AddUnit(Unit unit)
    {
        foreach (UnitSpawnPosition spawnPosition in _unitSpawnPositions)
        {
            if (spawnPosition.IsOccupied == false)
            {
                spawnPosition.Occupy();
                unit.transform.position = spawnPosition.transform.position;
                unit.TakeSpawnPositin(spawnPosition);
                unit.TakeBasePosition(transform.position);
                _units.Add(unit);

                if (IsResourcesEnough())
                {
                    _resources[ResourceType.Copper].DecreaseCount(_copperAmountForBuyUnit);
                    ResourceCountChanged?.Invoke(_resources);
                }

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

    public bool IsResourcesEnough()
    {
        if (_resources.ContainsKey(ResourceType.Copper))
            return _resources[ResourceType.Copper].Count >= _copperAmountForBuyUnit;
        else
            return false;
    }

    public Unit GetAvailableUnit()
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

    private void AddResource(ResourceType resourceType)
    {
        if (_resources.ContainsKey(resourceType) == false)
            NewResourceEntered?.Invoke(resourceType, this);

        _resources[resourceType].IncreaseCount();
        ResourceCountChanged?.Invoke(_resources);

        if (_resources.ContainsKey(ResourceType.Copper))
        {
            if (_resources[ResourceType.Copper].Count >= _copperAmountForBuyUnit)
            {
                if (HasFreeSpace())
                {
                    CopperEnough?.Invoke(this);
                }
            }
        }
    }

    private void SendUnitToBunner()
    {
        _resources[ResourceType.Iron].DecreaseCount(_ironAmountForBuyBase);
        ResourceCountChanged?.Invoke(_resources);
        _unitBaseCreator.MoveTo(_bunner.transform.position, _bunner);
    }

    private void OnUnitBaseCreated(Vector3 newBasePosition)
    {
        newBasePosition.y = transform.position.y;
        _unitBaseCreator.UnitBaseCreated -= OnUnitBaseCreated;
        _unitBaseCreator.StartPosition.FreeUp();
        UnitBaseCreated?.Invoke(newBasePosition, _unitBaseCreator, _bunner);
        _units.Remove(_unitBaseCreator);
        CanMoveBunner = true;
        CanBuyUnit = true;
        _bunner = null;
        _unitBaseCreator = null;
    }

    private IEnumerator WaitResource()
    {
        if (_resources.ContainsKey(ResourceType.Iron) == false)
        {
            while (_resources.ContainsKey(ResourceType.Iron) == false)
            {
                yield return null;
            }
        }

        while (_resources[ResourceType.Iron].Count < _ironAmountForBuyBase)
        {
            yield return null;
        }

        CanMoveBunner = false;
        SendUnitToBunner();
    }

    private IEnumerator WaitUnit()
    {
        while (_unitBaseCreator == null)
        {
            _unitBaseCreator = GetAvailableUnit();
            yield return null;
        }

        _unitBaseCreator.UnitBaseCreated += OnUnitBaseCreated;
        _unitBaseCreator.Occupy();
        CanBuyUnit = false;
        StartCoroutine(WaitResource());
    }
}