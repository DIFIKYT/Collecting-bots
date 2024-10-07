using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseUIManager : MonoBehaviour
{
    [SerializeField] private ButtonManager _buttonManager;
    [SerializeField] private TextManager _textManager;

    public ButtonManager ButtonManager => _buttonManager;

    public void OnChangeFoundResources(List<Resource> foundResources, Vector3 basePosition) =>
        _textManager.ChangeFoundResources(foundResources, basePosition);

    public void OnChangeResources(Dictionary<ResourceType, Counter> resources) =>
        _textManager.ChangeResources(resources);

    public void OnChangeUnitsInfo(List<Unit> units) =>
        _textManager.ChangeUnitsInfo(units);
}