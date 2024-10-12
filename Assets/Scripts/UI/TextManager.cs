using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;

    public void ChangeResourcesCount(Dictionary<ResourceType, Counter> resources)
    {
        _resourcesCount.text = null;

        foreach (ResourceType resourceType in resources.Keys)
            _resourcesCount.text += $"{resourceType}: {resources[resourceType].Count}\n";
    }
}