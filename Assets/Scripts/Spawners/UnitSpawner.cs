using System;
using UnityEngine;
using UnityEngine.Pool;

public class UnitSpawner : MonoBehaviour
{
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    [SerializeField] private Unit _unitPrefab;

    private ObjectPool<Unit> _pool;

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

    public Unit SpawnUnit()
    {
        return _pool.Get();
    }

    private Unit Create()
    {
        return Instantiate(_unitPrefab, transform);
    }

    private void OnGet(Unit unit)
    {
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
}