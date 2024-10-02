using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;
    [SerializeField] private TextMeshProUGUI _foundResources;
    [SerializeField] private TextMeshProUGUI _unitsInfo;

    public void ChangeResources(Dictionary<ResourceType, Counter> resources)
    {
        _resourcesCount.text = string.Empty;

        foreach (var resourceType in resources.Keys)
            _resourcesCount.text += $"{resourceType} count: {resources[resourceType].Count}\n";
    }

    public void ChangeFoundResources(List<Resource> foundResources)
    {
        _foundResources.text = null;

        foreach (Resource resource in foundResources)
        {
            _foundResources.text += $"{resource.Type}: distance from base " +
                $"{Mathf.Round(Vector3.Distance(transform.position, resource.transform.position))} \n";
        }
    }

    public void ChangeUnitsInfo(List<Unit> units)
    {
        int freeUnits = 0;
        int busyUnits = 0;

        foreach (Unit unit in units)
        {
            if (unit.IsBusy)
            {
                busyUnits++;
            }
            else
            {
                freeUnits++;
            }
        }

        _unitsInfo.text = $"Free units: {freeUnits}\nBusy units: {busyUnits}";
    }
}