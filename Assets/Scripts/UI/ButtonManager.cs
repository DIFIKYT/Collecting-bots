using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _scanButton;
    [SerializeField] private Button _sendUnit;
    [SerializeField] private UnitBase _unitBase;

    private void OnEnable()
    {
        _scanButton.onClick.AddListener(_unitBase.Scan);
        _sendUnit.onClick.AddListener(_unitBase.SendUnit);
    }

    private void OnDisable()
    {
        _scanButton.onClick.RemoveListener(_unitBase.Scan);
        _sendUnit.onClick.RemoveListener(_unitBase.SendUnit);
    }
}