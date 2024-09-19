using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _copperCount;
    [SerializeField] private TextMeshProUGUI _ironCount;
    [SerializeField] private TextMeshProUGUI _goldCount;
    [SerializeField] private TextMeshProUGUI _foundResources;
    [SerializeField] private TextMeshProUGUI _unitsInfo;

    private void Start()
    {
        _copperCount.text = "Copper count: 0";
        _ironCount.text = "Iron count: 0";
        _goldCount.text = "Gold count: 0";
        _foundResources.text = "Found resources: ";
        _unitsInfo.text = "Free units: 3\nBusy units: 0";
    }

    public void ChangeCopperCount(float copperCount)
    {
        _copperCount.text = "Copper count: " + copperCount;
    }

    public void ChangeIronCount(float ironCount)
    {
        _ironCount.text = "Iron count: " + ironCount;
    }

    public void ChangeGoldCount(float goldCount)
    {
        _goldCount.text = "Gold count: " + goldCount;
    }

    public void ChangeFoundResources(List<Resource> findResources)
    {
        foreach(Resource resource in findResources)
        {
            _foundResources.text += $"\n{resource.Name}: distance from base " +
                $"{Mathf.Abs(Vector3.Distance(transform.position, resource.transform.position))}";
        }
    }

    public void ChangeUnitsInfo(List<Unit> units)
    {
        int freeUnits = 0;
        int busyUnits = 0;

        foreach(Unit unit in units)
        {
            if(unit.IsBusy)
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