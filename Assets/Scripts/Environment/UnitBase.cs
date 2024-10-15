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
    private readonly int _minUnitsCountForBuildBase = 2;
    private Bunner _bunner;
    private Unit _unitBaseCreator;

    public event Action<Dictionary<ResourceType, Counter>> ResourceCountChanged;
    public event Action<Resource> ResourceEntered;
    public event Action<ResourceType, UnitBase> NewResourceEntered;
    public event Action<Vector3, Unit, Bunner> UnitBaseCreated;
    public event Action<Bunner> BunnerMoved;
    public event Action<UnitBase> CopperEnough;

    public bool CanMoveBunner { get; private set; }
    public bool CanBuyUnit { get; private set; }

    private void Awake()
    {
        CanBuyUnit = true;
        CanMoveBunner = true;
    }

    private void OnEnable()
    {
        ResourceEntered += OnResourceEntered;
    }

    private void OnDisable()
    {
        ResourceEntered -= OnResourceEntered;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            Resource resource = unit.Resource;

            if (resource != null && unit.IsResourceTaked && _units.Contains(unit))
            {
                //AddResource(resource.Type);
                ResourceEntered?.Invoke(unit.GetResource());
            }
        }
    }

    public void TakeBunner(Bunner bunner)
    {
        if (_bunner != null)
            BunnerMoved?.Invoke(_bunner);

        _bunner = bunner;
        StartCoroutine(WaitResource());
    }

    public void AddResourceToDictionary(ResourceType resourceType, Counter counter)
    {
        _resources.Add(resourceType, counter);
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

                return;
            }
        }
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

    private bool HasFreeSpace()
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

    private void OnResourceEntered(Resource resource)
    {
        if (_resources.ContainsKey(resource.Type) == false)
            NewResourceEntered?.Invoke(resource.Type, this);

        _resources[resource.Type].IncreaseCount();

        if (_resources.ContainsKey(ResourceType.Copper) && _resources[ResourceType.Copper].Count >= _copperAmountForBuyUnit)
        {
            if (HasFreeSpace())
            {
                _resources[ResourceType.Copper].DecreaseCount(_copperAmountForBuyUnit);
                CopperEnough?.Invoke(this);
            }
        }

        ResourceCountChanged?.Invoke(_resources);
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
        while (_resources.ContainsKey(ResourceType.Iron) == false || _resources[ResourceType.Iron].Count < _ironAmountForBuyBase)
            yield return null;

        CanMoveBunner = false;
        StartCoroutine(WaitUnit());
    }

    private IEnumerator WaitUnit()
    {
        while (_units.Count < _minUnitsCountForBuildBase)
            yield return null;

        while (_unitBaseCreator == null)
        {
            _unitBaseCreator = GetAvailableUnit();
            yield return null;
        }

        _unitBaseCreator.UnitBaseCreated += OnUnitBaseCreated;
        _unitBaseCreator.Occupy();
        CanBuyUnit = false;
        SendUnitToBunner();
    }
}