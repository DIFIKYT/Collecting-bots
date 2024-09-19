using System;
using UnityEngine;
using UnityEngine.Pool;

public class UnitSpawner : MonoBehaviour
{
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    [SerializeField] private Unit _unitPrefab;

    public event Action<Unit> UnitSpawned;

    private ObjectPool<Unit> _unitPool;
    private Vector3 _currentSpawnPosition;

    private void Awake()
    {
        _unitPool = new ObjectPool<Unit>(
            CreateUnit,
            OnGetUnit,
            OnReleaseUnit,
            DestroyUnit,
            collectionCheck: true,
            defaultCapacity: DefaultCapacity,
            maxSize: MaxSize);
    }

    public void Spawn(Vector3 spawnPosition)
    {
        _currentSpawnPosition = spawnPosition;
        _unitPool.Get();
    }

    private Unit CreateUnit()
    {
        Unit unit = Instantiate(_unitPrefab, transform);
        return unit;
    }

    private void OnGetUnit(Unit unit)
    {
        unit.transform.position = _currentSpawnPosition;
        UnitSpawned?.Invoke(unit);
        unit.gameObject.SetActive(true);
    }

    private void OnReleaseUnit(Unit unit)
    {
        unit.gameObject.SetActive(false);
    }

    private void DestroyUnit(Unit unit)
    {
        Destroy(unit.gameObject);
    }

    public void ReturnToPool(Unit unit)
    {
        if (!unit.IsBusy)
        {
            _unitPool.Release(unit);
        }
    }
}