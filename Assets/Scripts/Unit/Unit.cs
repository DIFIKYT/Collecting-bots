using UnityEngine;
using System;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitMover _unitMover;
    [SerializeField] private UnitConfig _config;
    [SerializeField] private UnitTrigger _unitTrigger;

    public event Action<Unit> WasFreed;

    private Resource _resource;
    private Vector3 _startPosition;
    private bool _isBusy;
    private bool _canTake;
    private const float ResourceOffsetDistance = 1.5f;

    public bool IsBusy => _isBusy;
    public bool CanTake => _canTake;
    public Resource Resource => _resource;

    private void Start() => 
        _startPosition = transform.position;

    private void OnEnable()
    {
        _unitTrigger.ResourceDetected += OnTakeResource;
    }

    private void OnDisable()
    {
        _unitTrigger.ResourceDetected -= OnTakeResource;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _canTake = true;
        _isBusy = true;

        targetPosition.y = _startPosition.y;

        MovementData movementData = new(transform, targetPosition, _config.BasePosition, _startPosition, _config.MoveSpeed);

        _unitMover.MoveQueue(movementData, () =>
        {
            _isBusy = false;
            WasFreed?.Invoke(this);
        });
    }

    public Resource GetResource()
    {
        Resource resourceForReturn = _resource;
        _resource = null;
        _isBusy = false;

        return resourceForReturn;
    }

    public void SetResource(Resource resource)
    {
        _resource = resource;
    }

    private void OnTakeResource(Resource resource)
    {
        if (_canTake == false || resource != _resource)
            return;

        _isBusy = true;
        _canTake = false;
        resource.transform.SetParent(transform, worldPositionStays: false);
        Vector3 offset = transform.position + transform.forward * ResourceOffsetDistance;
        resource.transform.SetPositionAndRotation(offset, transform.rotation);
    }
}