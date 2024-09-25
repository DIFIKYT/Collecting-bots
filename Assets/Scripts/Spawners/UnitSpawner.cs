using System;
using UnityEngine;
using UnityEngine.Pool;

public class UnitSpawner : MonoBehaviour
{
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    [SerializeField] private Unit _unitPrefab;

    public event Action<Unit> UnitSpawned;

    private ObjectPool<Unit> _pool;
    private Vector3 _spawnPosition;

    private void Awake()
    {
        _pool = new ObjectPool<Unit>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: DefaultCapacity,
            maxSize: MaxSize);
    }

    public void Spawn(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
        _pool.Get();
    }

    private Unit Create()
    {
        Unit unit = Instantiate(_unitPrefab, transform);

        return unit;
    }

    private void OnGet(Unit unit)
    {
        unit.transform.position = _spawnPosition;
        UnitSpawned?.Invoke(unit);
        unit.gameObject.SetActive(true);
    }

    private void OnRelease(Unit unit)
    {
        unit.gameObject.SetActive(false);
    }

    private void Destroy(Unit unit)
    {
        Destroy(unit.gameObject);
    }

    public void ReturnToPool(Unit unit)
    {
        if (unit.IsBusy == false)
        {
            _unitPool.Release(unit);
        }
    }
}