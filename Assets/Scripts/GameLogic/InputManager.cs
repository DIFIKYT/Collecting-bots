using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private BunnerBuilder _bunnerBuilder;

    private readonly int _minUnitsCountForPlaceBunner = 2;
    private UnitBase _selectedBase;

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedBase == null)
            {
                SelectBase();
            }
            else
            {
                if(_selectedBase.Units.Count >= _minUnitsCountForPlaceBunner && _selectedBase.CanMoveBunner)
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
        else if (Input.GetMouseButtonDown(1))
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