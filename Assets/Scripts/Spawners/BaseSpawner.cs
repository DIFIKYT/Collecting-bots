using System;
using UnityEngine;
using UnityEngine.Pool;

public class BaseSpawner : MonoBehaviour
{
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    [SerializeField] private UnitBase _unitBasePrefab;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private Vector3 _firstBaseSpawnCoordinate;

    public event Action<UnitBase> BaseSpawned;

    private ObjectPool<UnitBase> _pool;
    private Vector3 _spawnPosition;

    private void Awake()
    {
        _pool = new ObjectPool<UnitBase>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: DefaultCapacity,
            maxSize: MaxSize);
    }

    private void Start()
    {
        Spawn(_firstBaseSpawnCoordinate);
    }

    public void Spawn(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
        _pool.Get();
    }

    private UnitBase Create()
    {
        UnitBase unitBase = Instantiate(_unitBasePrefab, transform);
        unitBase.gameObject.SetActive(false);
        unitBase.TakeSpawners(_unitSpawner, _resourceSpawner);

        return unitBase;
    }



    private void OnGet(UnitBase unitBase)
    {
        unitBase.NewResourceEntered += GetDictionary;
        unitBase.transform.position = _spawnPosition;
        unitBase.gameObject.SetActive(true);
        BaseSpawned?.Invoke(unitBase);
    }

    private void OnRelease(UnitBase unitBase)
    {
        unitBase.NewResourceEntered -= GetDictionary;
        unitBase.transform.SetParent(transform, worldPositionStays: false);
        unitBase.gameObject.SetActive(false);
    }

    private void Destroy(UnitBase unitBase)
    {
        Destroy(unitBase.gameObject);
    }

    private void GetDictionary(ResourceType resourceType, UnitBase unitBase)
    {
        Counter _counter = new(0);
        unitBase.TakeNewDictionary(resourceType, _counter);
    }
}