using UnityEngine;

[CreateAssetMenu(fileName = "New unitConfig", menuName = "Unit/Create new unitConfig", order = 51)]
public class UnitConfig : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    public float MoveSpeed => _moveSpeed;
    public float RotateSpeed => _rotateSpeed;
}