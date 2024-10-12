using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(UnitMover))]
public class Unit : Spawnable
{
    private const float ResourceOffsetDistance = 1.5f;

    [SerializeField] private UnitMover _unitMover;
    [SerializeField] private UnitConfig _config;
    [SerializeField] private UnitTrigger _unitTrigger;

    private readonly float _timeCreateBase = 5;
    private bool _isBusy;
    private bool _canTake;
    private Vector3 _basePosition;
    private Resource _resource;

    public event Action<Unit> WasFreed;
    public event Action<Vector3> UnitBaseCreated;

    public UnitSpawnPosition StartPosition {  get; private set; }
    public Resource Resource => _resource;
    public bool IsBusy => _isBusy;
    public bool IsResourceTaked { get; private set; }

    private void OnEnable()
    {
        _unitTrigger.ResourceDetected += OnResourceDetected;
    }

    private void OnDisable()
    {
        _unitTrigger.ResourceDetected -= OnResourceDetected;
    }

    public void MoveTo(Vector3 targetPosition, Bunner bunner = null)
    {
        MovementData movementData;

        targetPosition.y = StartPosition.transform.position.y;
        movementData = new(transform, targetPosition, _basePosition, StartPosition.transform.position, _config.MoveSpeed, _config.RotateSpeed);

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
        IsResourceTaked = false;

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

    private void OnResourceDetected(Resource resource)
    {
        if (_canTake == false || resource != _resource)
            return;

        IsResourceTaked = true;
        _isBusy = true;
        _canTake = false;
        Vector3 offset = transform.position + transform.forward * ResourceOffsetDistance;
        resource.transform.SetParent(transform, worldPositionStays: false);
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