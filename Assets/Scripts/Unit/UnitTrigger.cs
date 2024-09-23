using System;
using UnityEngine;

public class UnitTrigger : MonoBehaviour
{
    public event Action<Resource> ResourceDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
            ResourceDetected?.Invoke(resource);
    }
}