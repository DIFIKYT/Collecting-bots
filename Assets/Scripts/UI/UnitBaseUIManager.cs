using System.Collections.Generic;
using UnityEngine;

public class UnitBaseUIManager : MonoBehaviour
{
    [SerializeField] private TextManager _textManager;

    public void OnResourceCountChanged(Dictionary<ResourceType, Counter> resources) =>
        _textManager.ChangeResourcesCount(resources);
}