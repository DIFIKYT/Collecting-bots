using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private TextViewer _textViewer;
    [SerializeField] private ButtonManager _buttonManager;
    [SerializeField] private List<Transform> _unitsSpawnPositions;
    [SerializeField] private float _scanRange;

    private float _copperCount;
    private float _ironCount;
    private float _goldCount;
    private List<Unit> _freeUnits = new();
    private List<Unit> _busyUnits = new();

    private void Start()
    {
        foreach (Transform spawnPosition in _unitsSpawnPositions)
        {
            Unit newUnit = Instantiate(_unitPrefab, spawnPosition.position, spawnPosition.rotation, transform);
            _freeUnits.Add(newUnit);
        }
    }
}