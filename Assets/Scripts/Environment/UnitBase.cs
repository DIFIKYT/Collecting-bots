using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
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
    }

    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            _unitSpawner.Spawn(_unitSpawnPositions[i].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit) && !unit.CanTake)
        {
            Resource resource = unit.GetComponentInChildren<Resource>();
            if (resource != null)
            {
                _resourceSpawner.ReturnToPool(resource);
                resource.SetIsTaked(false);
                resource.SetSelectedTarget(false);
                unit.SetCanTake(true);
            }
        }
    }

    private void ScanForResources()
    {
        _foundResources.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRange, LayerMask.GetMask("Resource"));

        foreach (Collider collider in colliders)
        {
            Resource resource = collider.GetComponent<Resource>();
            if (!resource.IsSelectedTarget)
            {
                _foundResources.Add(resource);
                Debug.Log($"{resource.Name} distance from base: {Vector3.Distance(transform.position, resource.transform.position)}");
            }
        }
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
    }

    private Unit GetAvailableUnit()
    {
        foreach (Unit unit in _units)
        {
            if (!unit.IsBusy)
            {
                return unit;
            }
        }
        return null;
    }

    private void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }
}