using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    private const float ResourceOffsetDistance = 1.5f;

    [SerializeField] private UnitMover _unitMover;
    [SerializeField] private UnitConfig _config;
    [SerializeField] private UnitTrigger _unitTrigger;

    public event Action<Unit> WasFreed;
    public event Action<Vector3> UnitBaseCreated;

    private readonly float _timeCreateBase = 5;
    private Resource _resource;
    //private UnitSpawnPosition _startPosition;
    private Vector3 _basePosition;
    private bool _isBusy;
    private bool _canTake;

    public UnitSpawnPosition StartPosition {  get; private set; }
    public bool IsBusy => _isBusy;

    private void OnEnable()
    {
        _unitTrigger.ResourceDetected += OnTakeResource;
    }

    private void OnDisable()
    {
        _unitTrigger.ResourceDetected -= OnTakeResource;
    }

    public void MoveTo(Vector3 targetPosition, Bunner bunner = null)
    {
        MovementData movementData;

        targetPosition.y = StartPosition.transform.position.y;
        movementData = new(transform, targetPosition, _basePosition, StartPosition.transform.position, _config.MoveSpeed);

        if (bunner != null)
        {
            _unitMover.MoveUnitToBunner(movementData, targetPosition, () =>
            {
                StartCoroutine(CreatingNewBase());
            });
        }
        else
        {
            _isBusy = true;
            _canTake = true;

            _unitMover.Move(movementData, () =>
            {
                _isBusy = false;
                WasFreed?.Invoke(this);
            });
        }
    }

    public Resource GetResource()
    {
        Resource resourceForReturn = _resource;
        _resource = null;

        return resourceForReturn;
    }

    public void SetResource(Resource resource)
    {
        _resource = resource;
    }

    public void TakeBasePosition(Vector3 basePosition)
    {
        _basePosition = basePosition;
        _basePosition.y = StartPosition.transform.position.y;
    }

    public void TakeSpawnPositin(UnitSpawnPosition startPosition)
    {
        StartPosition = startPosition;
    }

    public void Occupy()
    {
        _isBusy = true;
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

    private IEnumerator CreatingNewBase()
    {
        var delay = new WaitForSeconds(_timeCreateBase);
        _isBusy = true;

        yield return delay;

        _isBusy = false;
        UnitBaseCreated?.Invoke(transform.position);
    }
}