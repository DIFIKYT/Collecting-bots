using UnityEngine;
using UnityEngine.Pool;

public abstract class Spawner<T> : MonoBehaviour where T : Spawnable
{
    private const int DefaultCapacity = 100;
    private const int MaxSize = 100;

    protected ObjectPool<T> Pool { get; private set; }

    private void Awake()
    {
        Pool = new ObjectPool<T>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: DefaultCapacity,
            maxSize: MaxSize);
    }

    protected virtual void OnGet(T spawnable)
    {
        spawnable.gameObject.SetActive(true);
    }

    protected virtual void OnRelease(T spawnable)
    {
        spawnable.gameObject.SetActive(false);
    }

    protected void Destroy(T spawnable)
    {
        Destroy(spawnable.gameObject);
    }

    public void ReturnToPool(T spawnable)
    {
        Pool.Release(spawnable);
    }

    protected abstract T Create();
}