using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private const string ResourceName = "Resource";

    [SerializeField] private Unit _unitPrefab;
<<<<<<< HEAD
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
=======
    [SerializeField] private TextViewer _textViewer;
    [SerializeField] private ButtonManager _buttonManager;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private List<Transform> _unitSpawnPositions;
    [SerializeField] private float _scanRange;

    private float _copperCount;
    private float _ironCount;
    private float _goldCount;
    private int _startUnitCount = 3;
    private List<Resource> _foundResources = new();
    private List<Unit> _units = new();

    private void OnEnable()
    {
        _buttonManager.ScanButton.onClick.AddListener(ScanForResources);
        _buttonManager.SendUnitToResourceButton.onClick.AddListener(SendUnitToResource);
        _unitSpawner.UnitSpawned += AddUnit;
>>>>>>> parent of 6d15fef (Commit)
    }

    private void OnDisable()
    {
<<<<<<< HEAD
        if (_unitSpawner != null)
            _unitSpawner.UnitSpawned -= AddUnit;
=======
        _buttonManager.ScanButton.onClick.RemoveListener(ScanForResources);
        _buttonManager.SendUnitToResourceButton.onClick.RemoveListener(SendUnitToResource);
        _unitSpawner.UnitSpawned -= AddUnit;
>>>>>>> parent of 6d15fef (Commit)

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
        if (other.TryGetComponent(out Unit unit) && unit.CanTake == false)
        {
            Resource resource = unit.Resource;

            if (resource != null)
            {
                AddResource(resource.Type);

                _resourceSpawner.ReturnToPool(resource);
                resource.SetIsTaked(false);
                resource.SetSelectedTarget(false);
                unit.SetCanTake(true);
                unit.ClearResource();
            }
        }
    }

<<<<<<< HEAD
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
=======
    private void ScanForResources()
>>>>>>> parent of 6d15fef (Commit)
    {
        _foundResources.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRange, LayerMask.GetMask(ResourceName));

        foreach (Collider collider in colliders)
        {
            Resource resource = collider.GetComponent<Resource>();

            if (resource.IsSelectedTarget == false)
                _foundResources.Add(resource);
        }

        _textViewer.ChangeFoundResources(_foundResources);
    }

    private void SendUnitToResource()
    {
        if (_foundResources.Count <= 0)
            return;

        Unit freeUnit = GetAvailableUnit();

        if (freeUnit != null)
        {
            freeUnit.MoveTo(_foundResources[0].transform.position);
            _foundResources[0].SetSelectedTarget(true);
            _foundResources.RemoveAt(0);
            freeUnit.SetBusy(true);
        }

        _textViewer.ChangeUnitsInfo(_units);
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
        _textViewer.ChangeUnitsInfo(_units);
    }

    private void OnWasFreed(Unit unit)
    {
        _textViewer.ChangeUnitsInfo(_units);
    }
<<<<<<< HEAD
=======

    private void AddResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Copper:
                _copperCount++;
                _textViewer.ChangeCopperCount(_copperCount);
                break;
            case ResourceType.Iron:
                _ironCount++;
                _textViewer.ChangeIronCount(_ironCount);
                break;
            case ResourceType.Gold:
                _goldCount++;
                _textViewer.ChangeGoldCount(_goldCount);
                break;
        }
    }
>>>>>>> parent of 6d15fef (Commit)
}