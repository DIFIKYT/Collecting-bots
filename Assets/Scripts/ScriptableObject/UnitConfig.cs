using UnityEngine;

[CreateAssetMenu(fileName = "New unitConfig", menuName = "Unit/Create new unitConfig", order = 51)]
public class UnitConfig : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    //[SerializeField] private Vector3 _basePosition;

    public float MoveSpeed => _moveSpeed;
    //public Vector3 BasePosition => _basePosition;

    public Vector3 BasePosition { get; private set; }

    public void TakeBasePosition(Vector3 basePosition)
    {
        BasePosition = basePosition;
    }
}