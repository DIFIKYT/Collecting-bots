using System;
using System.Collections;
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
    public event Action<Vector3, Unit, Bunner> UnitBaseCreated;
    public event Action<Bunner> BunnerMoved;

    private readonly List<Resource> _foundResources = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly List<Unit> _units = new();
    private readonly Dictionary<ResourceType, Counter> _resources = new();
    private readonly int _copperAmountForBuyUnit = 3;
    private readonly int _ironAmountForBuyBase = 5;
    private ResourceSpawner _resourceSpawner;
    private Bunner _bunner;
    private Unit _unitBaseCreator;

    public int UnitsCount => _units.Count;
    public int Number { get; private set; }
    public bool CanMoveBunner {  get; private set; }
    public bool CanBuyUnit { get; private set; }

    private void Awake()
    {
        CanBuyUnit = true;
        CanMoveBunner = true;
    }

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
        BaseWasClicked?.Invoke(Number);
    }

    public void TakeBunner(Bunner bunner)
    {
        if (_bunner != null)
            BunnerMoved?.Invoke(_bunner);

        _bunner = bunner;
        StartCoroutine(WaitUnit());
    }

    public void TakeSpawners(ResourceSpawner resourceSpawner)
    {
        _resourceSpawner = resourceSpawner;
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
                unit.WasFreed += OnWasFreed;
                unit.TakeSpawnPositin(spawnPosition);
                unit.TakeBasePosition(transform.position);
                _units.Add(unit);

                if (IsResourcesEnough())
                {
                    _resources[ResourceType.Copper].DecreaseCount(_copperAmountForBuyUnit);
                    ResourceCountChanged?.Invoke(_resources);
                }

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

    public void SendUnitToResource()
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

    public bool IsResourcesEnough()
    {
        if (_resources.ContainsKey(ResourceType.Copper))
            return _resources[ResourceType.Copper].Count >= _copperAmountForBuyUnit;
        else
            return false;
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
        _unitBaseCreator.WasFreed -= OnWasFreed;
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