using System;
using UnityEngine;

public class BaseSpawner : Spawner<UnitBase>
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private Vector3 _firstBaseSpawnCoordinate;
    [SerializeField] private UnitBase _unitBasePrefab;

    private readonly int _unitsCountForFirstBase = 3;
    private bool _isFirstBase = true;
    private Vector3 _spawnPosition;

    public event Action<UnitBase> BaseSpawned;

    private void Start()
    {
        Spawn(_firstBaseSpawnCoordinate);
    }

    public UnitBase Spawn(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
        return Pool.Get();
    }

    protected override UnitBase Create()
    {
        return Instantiate(_unitBasePrefab, transform);
    }

    protected override void OnGet(UnitBase unitBase)
    {
        unitBase.NewResourceEntered += OnNewResourceEntered;
        unitBase.transform.position = _spawnPosition;
        base.OnGet(unitBase);
        BaseSpawned?.Invoke(unitBase);

        if (_isFirstBase)
        {
            for (int i = 0; i < _unitsCountForFirstBase; i++)
            {
                unitBase.AddUnit(_unitSpawner.SpawnUnit());
            }

            _isFirstBase = false;
        }
    }

    protected override void OnRelease(UnitBase unitBase)
    {
        unitBase.NewResourceEntered -= OnNewResourceEntered;
        unitBase.transform.SetParent(transform, worldPositionStays: false);
        base.OnRelease(unitBase);
    }

    private void OnNewResourceEntered(ResourceType resourceType, UnitBase unitBase)
    {
        Counter _counter = new(0);
        unitBase.AddResourceToDictionary(resourceType, _counter);
    }
}