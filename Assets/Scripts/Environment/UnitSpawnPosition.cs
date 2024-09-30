using UnityEngine;

public class UnitSpawnPosition : MonoBehaviour
{
    private bool _isOccupied;

    public bool IsOccupied => _isOccupied;

    public void FreeUp()=>
        _isOccupied = false;

    public void Occupy() =>
        _isOccupied = true;
}