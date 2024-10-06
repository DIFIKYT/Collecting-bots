using System;
using System.Collections;
using UnityEngine;

public class BunnerBuilder : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Bunner _bunnerPrefab;
    [SerializeField] private PreviewBunner _bunnerPreview;
    [SerializeField] private LayerMask _groundLayer;

    public event Action<Bunner> BunnerPlaced;

    private bool _isPlacing;

    private void Start()
    {
        _bunnerPreview.Disable();
    }

    public void PlaceBunnerPreview()
    {
        if (_isPlacing)
        {
            CancelPlacement();
            StopCoroutine(MoveBunnerPreview());
        }
        else
        {
            StartPlacement();
            StartCoroutine(MoveBunnerPreview());
        }
    }

    public void DestroyBunner(Bunner bunner)
    {
        Destroy(bunner.gameObject);
    }

    private void StartPlacement()
    {
        _isPlacing = true;
        _bunnerPreview.Enable();
    }

    private void CancelPlacement()
    {
        _isPlacing = false;
        _bunnerPreview.Disable();
    }

    private void PlaceBunner()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            CancelPlacement();
            Bunner bunner = Instantiate(_bunnerPrefab, hit.point, Quaternion.identity);
            BunnerPlaced?.Invoke(bunner);
        }
    }

    private IEnumerator MoveBunnerPreview()
    {
        Ray ray;
        RaycastHit hit;

        while (_isPlacing)
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
            {
                _bunnerPreview.transform.position = hit.point;
                MouseInput();
            }

            yield return null;
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBunner();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }
}