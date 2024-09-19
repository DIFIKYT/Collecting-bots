using UnityEngine;
using DG.Tweening;
using System;

public class Unit : MonoBehaviour
{
    private const float RotateDuration = 0.5f;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _basePosition;

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

        float toTargetDuration = Vector3.Distance(transform.position, targetPosition) / _moveSpeed;
        float toBaseDuration = Vector3.Distance(targetPosition, _basePosition) / _moveSpeed;
        float toStartDuration = Vector3.Distance(_basePosition, _startPosition) / _moveSpeed;

        targetPosition.y = _startPosition.y;
        _basePosition.y = _startPosition.y;

        transform.DOLookAt(targetPosition, RotateDuration).OnComplete(() =>
        {
            transform.DOMove(targetPosition, toTargetDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOLookAt(_basePosition, RotateDuration).OnComplete(() =>
                {
                    transform.DOMove(_basePosition, toBaseDuration).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        transform.DOLookAt(_startPosition, RotateDuration).OnComplete(() =>
                        {
                            transform.DOMove(_startPosition, toStartDuration).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                _isBusy = false;
                                WasFreed?.Invoke(this);
                            });
                        });
                    });
                });
            });
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