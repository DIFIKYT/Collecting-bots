using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;
    [SerializeField] private TextMeshProUGUI _foundResources;
    [SerializeField] private TextMeshProUGUI _unitsInfo;
<<<<<<< HEAD
    [SerializeField] private BaseSpawner _baseSpawner;
=======
>>>>>>> parent of 6d15fef (Commit)

    private UnitBase _unitBase;

    public void ChangeCopperCount(float copperCount)
    {
<<<<<<< HEAD
        _baseSpawner.BaseSpawned += OnBaseSpawned;
=======
        _copperCount.text = "Copper count: " + copperCount;
>>>>>>> parent of 6d15fef (Commit)
    }

    public void ChangeIronCount(float ironCount)
    {
<<<<<<< HEAD
        _baseSpawner.BaseSpawned -= OnBaseSpawned;
    }

    private void OnBaseSpawned(UnitBase unitBase)
    {
        _unitBase = unitBase;

        _unitBase.ResourceCountChanged += OnResourceChangeText;
        _unitBase.ResourcesFound += OnChangeFoundResources;
        _unitBase.UnitsCountChanged += OnChangeUnitsInfo;
=======
        _ironCount.text = "Iron count: " + ironCount;
>>>>>>> parent of 6d15fef (Commit)
    }

    public void ChangeGoldCount(float goldCount)
    {
        _goldCount.text = "Gold count: " + goldCount;
    }

    public void ChangeFoundResources(List<Resource> findResources)
    {
        _foundResources.text = null;

        foreach (Resource resource in findResources)
        {
            _foundResources.text += $"\n{resource.Type}: distance from base " +
                $"{Mathf.Round(Vector3.Distance(transform.position, resource.transform.position))}";
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
<<<<<<< HEAD

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
=======
>>>>>>> parent of 6d15fef (Commit)
}