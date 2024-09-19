using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _scanButton;
    [SerializeField] private Button _sendUnitToResourceButton;

    public Button ScanButton => _scanButton;
    public Button SendUnitToResourceButton => _sendUnitToResourceButton;
}