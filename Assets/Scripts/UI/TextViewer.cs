using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _copperCount;
    [SerializeField] private TextMeshProUGUI _ironCount;
    [SerializeField] private TextMeshProUGUI _goldCount;
    [SerializeField] private TextMeshProUGUI _foundResources;
    [SerializeField] private TextMeshProUGUI _unitsInfo;
    [SerializeField] private UnitBase _unitbase;

    private void Start()
    {
        _copperCount.text = "Copper count: 0";
        _ironCount.text = "Iron count: 0";
        _goldCount.text = "Gold count: 0";
        _unitsInfo.text = "Free units: 3\nBusy units: 0";
    }

    private void OnEnable()
    {
        _unitbase.ResourceCountChanged += OnResourceChangeText;
        _unitbase.ResourcesFound += OnChangeFoundResources;
        _unitbase.UnitsCountChanged += OnChangeUnitsInfo;
    }

    private void OnDisable()
    {
        _unitbase.ResourceCountChanged -= OnResourceChangeText;
        _unitbase.ResourcesFound -= OnChangeFoundResources;
        _unitbase.UnitsCountChanged -= OnChangeUnitsInfo;
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

    private void OnResourceChangeText(ResourceType resourceType, float resourceCount)
    {
        switch (resourceType)
        {
            case ResourceType.Copper:
                ChangeCopperText(resourceCount);
                break;
            case ResourceType.Iron:
                ChangeIronText(resourceCount);
                break;
            case ResourceType.Gold:
                ChangeGoldText(resourceCount);
                break;
        }
    }

    private void ChangeCopperText(float copperCount)
    {
        _copperCount.text = "Copper count: " + copperCount;
    }

    private void ChangeIronText(float ironCount)
    {
        _ironCount.text = "Iron count: " + ironCount;
    }

    private void ChangeGoldText(float goldCount)
    {
        _goldCount.text = "Gold count: " + goldCount;
    }
}