using UnityEngine;
using System;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _basePosition;
    [SerializeField] private UnitMover _unitMover;

    public event Action<Unit> WasFreed;

    private Vector3 _startPosition;
    private bool _isBusy = false;
    private bool _canTake = true;

    public bool IsBusy => _isBusy;
    public bool CanTake => _canTake;

    private void Start()
    {
        _startPosition = transform.position;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _isBusy = true;

        targetPosition.y = _startPosition.y;
        _basePosition.y = _startPosition.y;

        MovementData movementData = new(transform, targetPosition, _basePosition, _startPosition, _moveSpeed);

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
}