using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const int NumberLeftMouseButton = 0;
    private const int NumberRightMouseButton = 1;

    [SerializeField] private Camera _camera;
    [SerializeField] private BunnerBuilder _bunnerBuilder;

    private UnitBase _selectedBase;

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(NumberLeftMouseButton))
        {
            if (_selectedBase == null)
            {
                SelectBase();
            }
            else
            {
                if(_selectedBase.CanMoveBunner)
                {
                    if (_bunnerBuilder.IsPlacing)
                    {
                        _bunnerBuilder.PlaceBunner(_selectedBase);
                        _selectedBase = null;
                    }
                    else
                    {
                        _bunnerBuilder.StartPlacement();
                    }
                }
                else
                {
                    _selectedBase = null;
                }
            }
        }
        else if (Input.GetMouseButtonDown(NumberRightMouseButton))
        {
            _bunnerBuilder.CancelPlacement();
            _selectedBase = null;
        }
    }

    private void SelectBase()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out UnitBase unitBase))
            {
                _selectedBase = unitBase;
            }
        }
    }
}