using UnityEngine;

public struct MovementData
{
    public MovementData(Transform unitTransform, Vector3 targetPosition, Vector3 basePosition, Vector3 startPosition, float moveSpeed)
    {
        UnitTransform = unitTransform;
        TargetPosition = targetPosition;
        BasePosition = basePosition;
        StartPosition = startPosition;
        MoveSpeed = moveSpeed;
    }

    public Transform UnitTransform { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public Vector3 BasePosition { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public float MoveSpeed { get; private set; }
}