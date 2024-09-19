using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour
{
    [SerializeField] private string _name;

    private bool _isTaked = false;
    private bool _isSelectedTarget = false;

    public bool IsSelectedTarget => _isSelectedTarget;
    public string Name => _name;

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTaked == false)
        {
            if (_isSelectedTarget)
            {
                if (other.TryGetComponent(out Unit unit))
                {
                    _isTaked = true;
                    BecomfChildToUnit(this, unit);
                }
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

    private void BecomfChildToUnit(Resource resource, Unit unit)
    {
        if (unit.CanTake)
        {
            unit.SetCanTake(false);
            resource.transform.SetParent(unit.transform, worldPositionStays: false);
            Vector3 offset = unit.transform.position + unit.transform.forward * 1.5f;
            resource.transform.SetPositionAndRotation(offset, unit.transform.rotation);
        }
    }
}