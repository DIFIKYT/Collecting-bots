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
    public event Action<Vector3, Unit> BaseCreated;

    private readonly List<Resource> _foundResources = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly List<Unit> _units = new();
    private readonly Dictionary<ResourceType, Counter> _resources = new();
    private readonly int _copperAmountForBuyUnit = 3;
    private readonly int _ironAmountForBuyBase = 5;
    private readonly float _delay = 0.5f;
    private ResourceSpawner _resourceSpawner;
    private Bunner _bunner;
    private Unit _unitBaseCreator;

    public int Number { get; private set; }
    public bool IsBunnerPlaced { get; private set; }
    public bool CanBuyUnit { get; private set; }

    private void Awake()
    {
        CanBuyUnit = true;
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
        _bunner = bunner;
        IsBunnerPlaced = true;
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
                unit.BaseCreated += OnBaseCreated;
                unit.TakeSpawnPositin(spawnPosition.transform.position);
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

    private void ChangeState()
    {
        _unitBaseCreator = GetAvailableUnit();

        if (IsBunnerPlaced)
        {
            CanBuyUnit = false;

            StartCoroutine(WaitResource());
        }
    }

    private void SendUnitToBunner()
    {
        _resources[ResourceType.Iron].DecreaseCount(_ironAmountForBuyBase);
        _unitBaseCreator.MoveTo(_bunner.transform.position, _bunner);
    }

    private void OnBaseCreated(Vector3 newBasePosition)
    {
        BaseCreated?.Invoke(newBasePosition, _unitBaseCreator);
        IsBunnerPlaced = false;
        _bunner = null;
        ChangeState();
    }

    private IEnumerator WaitResource()
    {
        while (_resources[ResourceType.Iron].Count < _ironAmountForBuyBase)
        {
            yield return null;
        }

        SendUnitToBunner();
    }

    private IEnumerator WaitUnit()
    {
        var delay = new WaitForSeconds(_delay);

        while(_unitBaseCreator == null)
        {
            _unitBaseCreator = GetAvailableUnit();
            yield return delay;
        }

        _unitBaseCreator.Occupy();
        ChangeState();
    }
}