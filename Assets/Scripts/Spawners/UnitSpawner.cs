public class UnitSpawner : Spawner<Unit>
{
    public Unit SpawnUnit()
    {
        return Pool.Get();
    }
}