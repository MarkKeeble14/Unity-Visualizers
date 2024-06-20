using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizerSelectionGridGenerator : MonoBehaviour
{
    [SerializeField] private VisualizerSelectionButton buttonPrefab;

    [SerializeField] private List<VisualizerSelectionInfo> availableSelections = new();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        foreach (VisualizerSelectionInfo info in availableSelections)
        {
            VisualizerSelectionButton spawned = Instantiate(buttonPrefab, transform);
            spawned.SetSceneInfo(info);
        }
    }
}
