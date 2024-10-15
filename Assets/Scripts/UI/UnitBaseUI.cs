using System.Collections.Generic;
using UnityEngine;

public class UnitBaseUI : MonoBehaviour
{
    [SerializeField] private TextViewer _textManager;

    public void OnResourceCountChanged(Dictionary<ResourceType, Counter> resources) =>
        _textManager.ChangeResourcesCount(resources);
}