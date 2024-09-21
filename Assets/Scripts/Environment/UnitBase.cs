using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private const string ResourceName = "Resource";

    [SerializeField] private Unit _unitPrefab;
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
    }

    private void OnDisable()
    {
        _buttonManager.ScanButton.onClick.RemoveListener(ScanForResources);
        _buttonManager.SendUnitToResourceButton.onClick.RemoveListener(SendUnitToResource);
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

    private void ScanForResources()
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
}