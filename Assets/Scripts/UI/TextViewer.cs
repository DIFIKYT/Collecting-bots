using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;
    [SerializeField] private TextMeshProUGUI _foundResources;
    [SerializeField] private TextMeshProUGUI _unitsInfo;
    [SerializeField] private BaseSpawner _baseSpawner;

    private UnitBase _unitBase;

    private void OnEnable()
    {
        _baseSpawner.BaseSpawned += OnBaseSpawned;
    }

    private void OnDisable()
    {
        _baseSpawner.BaseSpawned -= OnBaseSpawned;
    }

    private void OnBaseSpawned(UnitBase unitBase)
    {
        _unitBase = unitBase;

        _unitBase.ResourceCountChanged += OnResourceChangeText;
        _unitBase.ResourcesFound += OnChangeFoundResources;
        _unitBase.UnitsCountChanged += OnChangeUnitsInfo;
    }

    private void OnChangeUnitsInfo(List<Unit> units)
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

    private void OnChangeFoundResources(List<Resource> findResources)
    {
        _foundResources.text = null;

        foreach (Resource resource in findResources)
        {
            _foundResources.text += $"\n{resource.Type}: distance from base " +
                $"{Mathf.Round(Vector3.Distance(transform.position, resource.transform.position))}";
        }
    }

    private void OnResourceChangeText(Dictionary<ResourceType, Counter> resources)
    {
        _resourcesCount.text = string.Empty;
        foreach (var resourceType in resources.Keys)
            _resourcesCount.text += $"{resourceType} count: {resources[resourceType].Count}\n";
    }
}