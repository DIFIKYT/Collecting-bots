using UnityEngine;

[CreateAssetMenu(fileName = "New unitConfig", menuName = "Unit/Create new unitConfig", order = 51)]
public class UnitConfig : ScriptableObject
{
    [SerializeField] private float _moveSpeed;

    public float MoveSpeed => _moveSpeed;
}