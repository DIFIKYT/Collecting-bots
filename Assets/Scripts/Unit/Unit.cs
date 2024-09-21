using UnityEngine;
using System;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitMover _unitMover;
    [SerializeField] private UnitConfig _config;

    public event Action<Unit> WasFreed;

    private Resource _resource;
    private Vector3 _startPosition;
    private bool _isBusy = false;
    private bool _canTake = true;

    public bool IsBusy => _isBusy;
    public bool CanTake => _canTake;
    public Resource Resource => _resource;

    private void Start()
    {
        _startPosition = transform.position;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _isBusy = true;

        targetPosition.y = _startPosition.y;

        MovementData movementData = new(transform, targetPosition, _config.BasePosition, _startPosition, _config.MoveSpeed);

        _unitMover.MoveQueue(movementData, () =>
        {
            _isBusy = false;
            WasFreed?.Invoke(this);
        });
    }

    public void SetCanTake(bool canTake)
    {
        _canTake = canTake;
    }

    public void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
    }

    public void TakeResource(Resource resource)
    {
        _resource = resource;
    }

    public void ClearResource()
    {
        _resource = null;
    }
}