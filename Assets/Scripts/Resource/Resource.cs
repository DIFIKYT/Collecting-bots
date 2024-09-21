using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _type;

    private bool _isTaked;
    private bool _isSelectedTarget;
    private const float ResourceOffsetDistance = 1.5f;

    public bool IsSelectedTarget => _isSelectedTarget;
    public ResourceType Type => _type;

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTaked == false)
        {
            if (_isSelectedTarget && other.TryGetComponent(out Unit unit))
            {
                _isTaked = true;
                BecomeChildToUnit(this, unit);
            }
        }
    }

    public void SetSelectedTarget(bool isSelectedTarget)
    {
        _isSelectedTarget = isSelectedTarget;
    }

    public void SetIsTaked(bool isTaked)
    {
        _isTaked = isTaked;
    }

    private void BecomeChildToUnit(Resource resource, Unit unit)
    {
        if (unit.CanTake)
        {
            unit.TakeResource(resource);
            unit.SetCanTake(false);
            resource.transform.SetParent(unit.transform, worldPositionStays: false);
            Vector3 offset = unit.transform.position + unit.transform.forward * ResourceOffsetDistance;
            resource.transform.SetPositionAndRotation(offset, unit.transform.rotation);
        }
    }
}