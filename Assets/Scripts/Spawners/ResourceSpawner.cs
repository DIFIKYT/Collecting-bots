using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    private const float ScaleDivisionFactor = 2.0f;
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    [SerializeField] private List<Resource> _resourcePrefabs;
    [SerializeField] private Ground _ground;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnHeight;

    private ObjectPool<Resource> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Resource>(
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
        StartCoroutine(SpawnCoroutine());
    }

    public void ReturnToPool(Resource resource)
    {
        _pool.Release(resource);
    }

    private Resource Create()
    {
        Resource resource = Instantiate(_resourcePrefabs[Random.Range(0, _resourcePrefabs.Count)], transform);
        resource.enabled = true;
        return resource;
    }

    private void OnGet(Resource resource)
    {
        resource.transform.position = GetSpawnPosition();
        resource.gameObject.SetActive(true);
    }

    private void OnRelease(Resource resource)
    {
        resource.transform.SetParent(transform, worldPositionStays: false);
        resource.gameObject.SetActive(false);
    }

    private void Destroy(Resource resource)
    {
        Destroy(resource.gameObject);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 currentSpawnPosition;
        bool isPositionFree;
        float randomXPosition;
        float randomZPosition;
        float lengthRadiusCheck = 1f;
        float halfScaleXGround = _ground.transform.localScale.x / ScaleDivisionFactor;
        float halfScaleZGround = _ground.transform.localScale.z / ScaleDivisionFactor;

        do
        {
            randomXPosition = Random.Range(_ground.transform.position.x - halfScaleXGround, _ground.transform.position.x + halfScaleXGround);
            randomZPosition = Random.Range(_ground.transform.position.z - halfScaleZGround, _ground.transform.position.z + halfScaleZGround);

            currentSpawnPosition = new Vector3(randomXPosition, _spawnHeight, randomZPosition);

            isPositionFree = !Physics.CheckSphere(currentSpawnPosition, lengthRadiusCheck, LayerMask.GetMask("Resource", "UnitBase"));

        } while (isPositionFree == false);

        return currentSpawnPosition;
    }

    private IEnumerator SpawnCoroutine()
    {
        var spawnDelay = new WaitForSeconds(_spawnDelay);

        while (true)
        {
            if (_pool != null)
                _pool.Get();

            yield return spawnDelay;
        }
    }
}