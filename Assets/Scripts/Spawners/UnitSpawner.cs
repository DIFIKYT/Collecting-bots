using UnityEngine;

public class UnitSpawner : Spawner<Unit>
{
    [SerializeField] private Unit _unitPrefab;

    public Unit SpawnUnit()
    {
        return Pool.Get();
    }

    protected override Unit Create()
    {
        return Instantiate(_unitPrefab, transform);
    }
}