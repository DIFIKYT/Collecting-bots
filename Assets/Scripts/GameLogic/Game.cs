using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private UnitBaseUIManager _unitBaseUIPrefab;
    [SerializeField] private Canvas _canvas;

    private readonly Dictionary<int, UnitBaseUIManager> _unitBasesUI = new();
    private readonly Dictionary<UnitBaseUIManager, UnitBase> _unitBases = new();
    private UnitBaseUIManager _currentUnitBaseUI;
    private UnitBase _currentUnitBase;

    private void OnEnable()
    {
        _baseSpawner.BaseSpawned += OnUnitBaseSpawned;
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
        }

        foreach (UnitBase unitBase in _unitBases.Values)
        {
            unitBase.BaseWasClicked -= ActiveUnitBase;
        }
    }

    private void OnUnitBaseSpawned(UnitBase unitBase)
    {
        UnitBaseUIManager unitBaseUIManager = Instantiate(_unitBaseUIPrefab, _canvas.transform);
        unitBaseUIManager.gameObject.SetActive(false);

        _unitBasesUI.Add(unitBase.Number, unitBaseUIManager);
        _unitBases.Add(unitBaseUIManager, unitBase);

        unitBase.BaseWasClicked += ActiveUnitBase;
        unitBase.ResourceCountChanged += unitBaseUIManager.OnChangeResources;
        unitBase.ResourcesFound += unitBaseUIManager.OnChangeFoundResources;
        unitBase.UnitsCountChanged += unitBaseUIManager.OnChangeUnitsInfo;

        unitBaseUIManager.ButtonManager.ScanButton.onClick.AddListener(OnScanButtonPressed);
        unitBaseUIManager.ButtonManager.SendUnitButton.onClick.AddListener(OnSendUnitButtonPressed);
        unitBaseUIManager.ButtonManager.BuyUnitButton.onClick.AddListener(OnBuyUnitButtonPressed);
    }

    private void ActiveUnitBase(int unitBaseNumber)
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
        _currentUnitBase.SendUnit();
    }

    private void OnBuyUnitButtonPressed()
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