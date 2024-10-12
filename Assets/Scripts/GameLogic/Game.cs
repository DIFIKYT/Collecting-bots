using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private BunnerBuilder _bunnerBuilder;
    [SerializeField] private UnitBaseUIManager _unitBaseUIManagerPrefab;
    [SerializeField] private Scaner _scaner;
    [SerializeField] private Canvas _canvas;

    private readonly Dictionary<UnitBase, UnitBaseUIManager> _unitBasesUI = new();
    private readonly List<Resource> _selectedResources = new();
    private readonly float _delay = 1;

    private void OnEnable()
    {
        _baseSpawner.BaseSpawned += OnUnitBaseSpawned;
        _bunnerBuilder.BunnerPlaced += OnBunnerPlaced;
    }

    private void OnDisable()
    {
        _baseSpawner.BaseSpawned -= OnUnitBaseSpawned;

        foreach (UnitBase unitBase in _unitBasesUI.Keys)
        {
            unitBase.ResourceCountChanged -= _unitBasesUI[unitBase].OnResourceCountChanged;
            unitBase.ResourceEntered -= OnResourceEnetered;
            unitBase.UnitBaseCreated -= OnUnitBaseCreated;
            unitBase.BunnerMoved -= OnBunnerMove;
            unitBase.CopperEnough -= OnCopperEnough;
        }
    }

    private void Start()
    {
        StartCoroutine(SendUnitToResources());
    }

    private IEnumerator SendUnitToResources()
    {
        var delay = new WaitForSeconds(_delay);
        Unit freeUnit;

        while (true)
        {
            var resources = _scaner.Scan();

            foreach (Resource resource in resources)
            {
                if (_selectedResources.Contains(resource) == false)
                {
                    foreach (UnitBase unitBase in _unitBasesUI.Keys)
                    {
                        freeUnit = unitBase.GetAvailableUnit();

                        if (freeUnit != null)
                        {
                            freeUnit.MoveTo(resource.transform.position);
                            freeUnit.SetResource(resource);
                            _selectedResources.Add(resource);
                        }
                    }
                }
            }

            yield return delay;
        }
    }

    private void OnUnitBaseSpawned(UnitBase unitBase)
    {
        Vector3 positionUI = new(unitBase.transform.position.x, unitBase.transform.position.y+6, unitBase.transform.position.z);

        UnitBaseUIManager unitBaseUIManager = Instantiate(_unitBaseUIManagerPrefab, positionUI, unitBase.transform.rotation, _canvas.transform);

        _unitBasesUI.Add(unitBase, unitBaseUIManager);

        unitBase.ResourceCountChanged += _unitBasesUI[unitBase].OnResourceCountChanged;
        unitBase.ResourceEntered += OnResourceEnetered;
        unitBase.UnitBaseCreated += OnUnitBaseCreated;
        unitBase.BunnerMoved += OnBunnerMove;
        unitBase.CopperEnough += OnCopperEnough;
    }

    private void OnResourceEnetered(Resource resource)
    {
        _resourceSpawner.ReturnToPool(resource);
        _selectedResources.Remove(resource);
    }

    private void OnBunnerPlaced(UnitBase unitBase, Bunner bunner)
    {
        unitBase.TakeBunner(bunner);
    }

    private void OnBunnerMove(Bunner bunner)
    {
        _bunnerBuilder.DestroyBunner(bunner);
    }

    private void OnUnitBaseCreated(Vector3 newBaseSpawnPosition, Unit unit, Bunner bunner)
    {
        UnitBase unitBase = _baseSpawner.Spawn(newBaseSpawnPosition);
        unitBase.AddUnit(unit);
        _bunnerBuilder.DestroyBunner(bunner);
    }

    private void OnCopperEnough(UnitBase unitBase)
    {
        unitBase.AddUnit(_unitSpawner.SpawnUnit());
    }
}