using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;

    public void ChangeResourcesCount(Dictionary<ResourceType, Counter> resources)
    {
        _resourcesCount.text = null;

        foreach (ResourceType resourceType in resources.Keys)
        {
            if(resourceType == ResourceType.Copper)
            {
                _resourcesCount.text += $"Copper: {resources[resourceType].Count}\n";
            }
            else if (resourceType == ResourceType.Iron)
            {
                _resourcesCount.text += $"Iron: {resources[resourceType].Count}\n";
            }
        }
    }
}