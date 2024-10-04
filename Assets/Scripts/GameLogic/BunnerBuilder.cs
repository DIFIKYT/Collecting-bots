using UnityEngine;

public class BunnerBuilder : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Bunner _flagPrefab;
    [SerializeField] private BunnerPreview _flagPreviewPrefab;

    private readonly int _distance = 100;
    private Transform _objectHits;
    private Ray _ray;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(_ray.origin, _ray.direction * _distance, Color.red);

        if (Physics.Raycast(_ray, out RaycastHit hit, Mathf.Infinity))
        {
            //if(hit.transform.TryGetComponent(out Ground _))
            //{
            //    _objectHits = hit.transform;
            //}

            _objectHits = hit.transform;
            //print(objectHits);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Transform objectHit in _objectHits)
            {
                if (objectHit.TryGetComponent(out Ground _))
                {
                    if (_flagPreviewPrefab.IsActive == false)
                    {
                        _flagPreviewPrefab.Enable();
                        _flagPreviewPrefab.SetPosition(objectHit.transform.position);
                    }
                }
            }
        }
    }

    private void Build()
    {

    }
}