using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _scanButton;
    [SerializeField] private Button _sendUnitButton;
    [SerializeField] private BaseSpawner _baseSpawner;

    private UnitBase _unitBase;

    private void OnEnable()
    {
        _baseSpawner.BaseSpawned += OnBaseSpawned;
    }

    private void OnDisable()
    {
        _baseSpawner.BaseSpawned -= OnBaseSpawned;

        if (_unitBase != null)
        {
            _scanButton.onClick.RemoveListener(_unitBase.Scan);
            _sendUnitButton.onClick.RemoveListener(_unitBase.SendUnit);
        }
    }

    private void OnBaseSpawned(UnitBase unitBase)
    {
        _unitBase = unitBase;

        _scanButton.onClick.AddListener(_unitBase.Scan);
        _sendUnitButton.onClick.AddListener(_unitBase.SendUnit);
    }
}