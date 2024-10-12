using System.Collections.Generic;
using UnityEngine;

public class Scaner : MonoBehaviour
{
    [SerializeField] private LayerMask _resourceLayer;

    private readonly float _scanRange = 100;

    public IEnumerable<Resource> Scan()
    {
        List<Resource> resources = new();

        foreach (var collider in Physics.OverlapSphere(transform.position, _scanRange, _resourceLayer))
            if (collider.TryGetComponent(out Resource resource))
                resources.Add(resource);

        return resources;
    }
}