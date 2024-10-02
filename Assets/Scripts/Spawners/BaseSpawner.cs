using System;
using Unity.VisualScripting;
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

    private readonly int _unitsCountForFirstBase = 3;
    private bool _isFirstBase = true;
    public event Action<UnitBase> BaseSpawned;

    private ObjectPool<UnitBase> _pool;
    private Vector3 _spawnPosition;
    private int _unitBaseNumber;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Spawn(new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50)));
        }
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
        unitBase.TakeNumber(_unitBaseNumber);

        _unitBaseNumber++;

        return unitBase;
    }

    private void OnGet(UnitBase unitBase)
    {
        unitBase.NewResourceEntered += GetDictionary;
        unitBase.transform.position = _spawnPosition;
        unitBase.gameObject.SetActive(true);
        BaseSpawned?.Invoke(unitBase);

        if (_isFirstBase)
        {
            unitBase.SpawnUnits(_unitsCountForFirstBase);
            _isFirstBase = false;
        }
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