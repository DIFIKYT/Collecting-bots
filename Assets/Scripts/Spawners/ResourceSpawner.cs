using System.Collections;
using UnityEngine;

public class ResourceSpawner : Spawner<Resource>
{
    private const string ResourceName = "Resource";
    private const string UnitBaseName = "UnitBase";
    private const float ScaleDivisionFactor = 2.0f;

    [SerializeField] private Ground _ground;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnHeight;

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    protected override Resource Create()
    {
        return Instantiate(Prefabs[Random.Range(0, Prefabs.Count)], transform);
    }

    protected override void OnGet(Resource resource)
    {
        resource.transform.position = GetSpawnPosition();
        base.OnGet(resource);
    }

    protected override void OnRelease(Resource resource)
    {
        resource.transform.SetParent(transform, worldPositionStays: false);
        base.OnRelease(resource);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 currentSpawnPosition;
        bool isPositionFree;
        float randomXPosition;
        float randomZPosition;
        float lengthRadiusCheck = 5f;
        float halfScaleXGround = _ground.transform.localScale.x / ScaleDivisionFactor;
        float halfScaleZGround = _ground.transform.localScale.z / ScaleDivisionFactor;

        do
        {
            randomXPosition = Random.Range(_ground.transform.position.x - halfScaleXGround, _ground.transform.position.x + halfScaleXGround);
            randomZPosition = Random.Range(_ground.transform.position.z - halfScaleZGround, _ground.transform.position.z + halfScaleZGround);

            currentSpawnPosition = new Vector3(randomXPosition, _spawnHeight, randomZPosition);

            isPositionFree = !Physics.CheckSphere(currentSpawnPosition, lengthRadiusCheck, LayerMask.GetMask(ResourceName, UnitBaseName));

        } while (isPositionFree == false);

        return currentSpawnPosition;
    }

    private IEnumerator SpawnCoroutine()
    {
        var spawnDelay = new WaitForSeconds(_spawnDelay);

        while (true)
        {
            if (Pool != null)
                Pool.Get();

            yield return spawnDelay;
        }
    }
}