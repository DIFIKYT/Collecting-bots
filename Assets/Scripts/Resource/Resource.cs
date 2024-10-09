using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : Spawnable
{
    [SerializeField] private ResourceType _type;

    public ResourceType Type => _type;

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}