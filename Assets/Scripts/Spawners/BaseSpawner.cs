using System;
using UnityEngine;

public class BaseSpawner : Spawner<UnitBase>
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private Vector3 _firstBaseSpawnCoordinate;

    private readonly int _unitsCountForFirstBase = 3;
    private bool _isFirstBase = true;
    private int _unitBaseNumber;
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
        UnitBase unitBase = base.Create();

        unitBase.TakeSpawners(_resourceSpawner);
        unitBase.TakeNumber(_unitBaseNumber);

        _unitBaseNumber++;

        return unitBase;
    }

    protected override void OnGet(UnitBase unitBase)
    {
        unitBase.NewResourceEntered += GetDictionary;
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
        unitBase.NewResourceEntered -= GetDictionary;
        unitBase.transform.SetParent(transform, worldPositionStays: false);
        base.OnRelease(unitBase);
    }

    private void GetDictionary(ResourceType resourceType, UnitBase unitBase)
    {
        Counter _counter = new(0);
        unitBase.TakeDictionary(resourceType, _counter);
    }
}