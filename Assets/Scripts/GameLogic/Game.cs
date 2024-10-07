using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private UnitBaseUIManager _unitBaseUIPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private BunnerBuilder _bunnerBuilder;

    private readonly Dictionary<int, UnitBaseUIManager> _unitBasesUI = new();
    private readonly Dictionary<UnitBaseUIManager, UnitBase> _unitBases = new();
    private readonly int _minUnitsCountForPlaceBunner = 2;
    private UnitBaseUIManager _currentUnitBaseUI;
    private UnitBase _currentUnitBase;

    private void OnEnable()
    {
        _baseSpawner.BaseSpawned += OnUnitBaseSpawned;
        _bunnerBuilder.BunnerPlaced += OnBunnerPlaced;
    }

    private void OnDisable()
    {
        _baseSpawner.BaseSpawned -= OnUnitBaseSpawned;

        foreach (UnitBaseUIManager unitBaseUIManager in _unitBasesUI.Values)
        {
            ButtonManager buttonManager = unitBaseUIManager.ButtonManager;
            buttonManager.ScanButton.onClick.RemoveListener(OnScanButtonPressed);
            buttonManager.SendUnitButton.onClick.RemoveListener(OnSendUnitButtonPressed);
            buttonManager.BuyUnitButton.onClick.RemoveListener(OnBuyUnitButtonPressed);
            buttonManager.SetBunnerButton.onClick.RemoveListener(OnPlaceBunnerButtonPressed);
        }

        foreach (UnitBase unitBase in _unitBases.Values)
        {
            unitBase.BaseWasClicked -= OnBaseWasClicked;
            unitBase.UnitBaseCreated -= OnUnitBaseCreated;
        }
    }

    private void OnUnitBaseSpawned(UnitBase unitBase)
    {
        UnitBaseUIManager unitBaseUIManager = Instantiate(_unitBaseUIPrefab, _canvas.transform);
        unitBaseUIManager.gameObject.SetActive(false);

        _unitBasesUI.Add(unitBase.Number, unitBaseUIManager);
        _unitBases.Add(unitBaseUIManager, unitBase);

        unitBase.BaseWasClicked += OnBaseWasClicked;
        unitBase.ResourceCountChanged += unitBaseUIManager.OnChangeResources;
        unitBase.ResourcesFound += unitBaseUIManager.OnChangeFoundResources;
        unitBase.UnitsCountChanged += unitBaseUIManager.OnChangeUnitsInfo;
        unitBase.UnitBaseCreated += OnUnitBaseCreated;
        unitBase.BunnerMoved += OnBunnerMove;

        unitBaseUIManager.ButtonManager.ScanButton.onClick.AddListener(OnScanButtonPressed);
        unitBaseUIManager.ButtonManager.SendUnitButton.onClick.AddListener(OnSendUnitButtonPressed);
        unitBaseUIManager.ButtonManager.BuyUnitButton.onClick.AddListener(OnBuyUnitButtonPressed);
        unitBaseUIManager.ButtonManager.SetBunnerButton.onClick.AddListener(OnPlaceBunnerButtonPressed);
    }

    private void OnBaseWasClicked(int unitBaseNumber)
    {
        if (_currentUnitBaseUI != null)
            _currentUnitBaseUI.gameObject.SetActive(false);

        _currentUnitBaseUI = _unitBasesUI[unitBaseNumber];
        _currentUnitBase = _unitBases[_currentUnitBaseUI];
        _currentUnitBaseUI.gameObject.SetActive(true);
    }

    private void OnScanButtonPressed()
    {
        _currentUnitBase.Scan();
    }

    private void OnSendUnitButtonPressed()
    {
        _currentUnitBase.SendUnitToResource();
    }

    private void OnBuyUnitButtonPressed()
    {
        if (_currentUnitBase.CanBuyUnit)
        {
            if (_currentUnitBase.IsResourcesEnough())
            {
                if (_currentUnitBase.HasFreeSpace())
                {
                    _currentUnitBase.AddUnit(_unitSpawner.SpawnUnit());
                }
                else
                {
                    Debug.Log("No free positions");
                }
            }
        }
    }

    private void OnPlaceBunnerButtonPressed()
    {
        if (_currentUnitBase.CanMoveBunner && _currentUnitBase.UnitsCount >= _minUnitsCountForPlaceBunner)
        {
            _bunnerBuilder.PlaceBunnerPreview();
        }
    }

    private void OnBunnerPlaced(Bunner bunner)
    {
        _currentUnitBase.TakeBunner(bunner);
    }

    private void OnUnitBaseCreated(Vector3 newBaseSpawnPosition, Unit unit, Bunner bunner)
    {
        UnitBase unitBase = _baseSpawner.Spawn(newBaseSpawnPosition);
        unitBase.AddUnit(unit);
        _bunnerBuilder.DestroyBunner(bunner);
    }

    private void OnBunnerMove(Bunner bunner)
    {
        _bunnerBuilder.DestroyBunner(bunner);
    }
}