using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _scanButton;
    [SerializeField] private Button _sendUnitButton;
    [SerializeField] private Button _buyUnitButton;
    //[SerializeField] private Button _setFlagButton;

    public Button ScanButton => _scanButton;
    public Button SendUnitButton => _sendUnitButton;
    public Button BuyUnitButton => _buyUnitButton;
    //public Button SetFlagButton => _setFlagButton;
}