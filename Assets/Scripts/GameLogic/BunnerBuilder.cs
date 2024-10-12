using System;
using System.Collections;
using UnityEngine;

public class BunnerBuilder : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Bunner _bunnerPrefab;
    [SerializeField] private PreviewBunner _bunnerPreview;
    [SerializeField] private LayerMask _groundLayer;

    public bool IsPlacing { get; private set; }

    public event Action<UnitBase, Bunner> BunnerPlaced;

    private void Start()
    {
        _bunnerPreview.Disable();
    }

    public void DestroyBunner(Bunner bunner)
    {
        Destroy(bunner.gameObject);
    }

    public void StartPlacement()
    {
        IsPlacing = true;
        _bunnerPreview.Enable();
        StartCoroutine(MoveBunnerPreview());
    }

    public void CancelPlacement()
    {
        IsPlacing = false;
        _bunnerPreview.Disable();
        StopCoroutine(MoveBunnerPreview());
    }

    public void PlaceBunner(UnitBase unitBase)
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            CancelPlacement();
            Bunner bunner = Instantiate(_bunnerPrefab, hit.point, Quaternion.identity);
            BunnerPlaced?.Invoke(unitBase, bunner);
        }
    }

    private IEnumerator MoveBunnerPreview()
    {
        Ray ray;

        while (IsPlacing)
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                _bunnerPreview.transform.position = hit.point;
            }

            yield return null;
        }
    }
}