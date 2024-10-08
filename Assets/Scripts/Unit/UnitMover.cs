using System;
using System.Collections;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    private const float RotateSpeed = 360f;
    private const float DistanceFromTargetPosition = 0.01f;

    public void MoveQueue(MovementData movementData, Action onComplete)
    {
        StartCoroutine(MoveAndRotateQueue(movementData, onComplete));
    }

    private IEnumerator MoveAndRotateQueue(MovementData movementData, Action onComplete)
    {
        yield return RotateAndMove(movementData, movementData.TargetPosition);
        yield return RotateAndMove(movementData, movementData.BasePosition);
        yield return RotateAndMove(movementData, movementData.StartPosition);

        onComplete?.Invoke();
    }

    private IEnumerator RotateAndMove(MovementData movementData, Vector3 targetPosition)
    {
        yield return RotateTo(movementData.UnitTransform, targetPosition);
        yield return MoveTo(movementData.UnitTransform, targetPosition, movementData.MoveSpeed);
    }

    private IEnumerator MoveTo(Transform unitTransform, Vector3 targetPosition, float moveSpeed)
    {
        while (Vector3.Distance(unitTransform.position, targetPosition) > DistanceFromTargetPosition)
        {
            unitTransform.position = Vector3.MoveTowards(unitTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator RotateTo(Transform unitTransform, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - unitTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(unitTransform.rotation, targetRotation) > DistanceFromTargetPosition)
        {
            unitTransform.rotation = Quaternion.RotateTowards(unitTransform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}