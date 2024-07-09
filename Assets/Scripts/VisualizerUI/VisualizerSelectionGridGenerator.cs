using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerSelectionGridGenerator : MonoBehaviour
{
    [SerializeField] private VisualizerSelectionButton buttonPrefab;
    [SerializeField] private List<SerializableKeyValuePair<string, VisualizerSelectionInfo>> allSelections = new();
    private List<VisualizerSelectionInfo> currentlyAvailableSelections = new();
    [SerializeField] private string[] defaultSelections;

    public static VisualizerSelectionGridGenerator _Instance { get; private set; }

    [SerializeField] private Button backButton;

    public Stack<string[]> selectionsStack = new();

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSelection(defaultSelections, true);
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Clear
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Generate
        foreach (VisualizerSelectionInfo info in currentlyAvailableSelections)
        {
            VisualizerSelectionButton spawned = Instantiate(buttonPrefab, transform);
            spawned.SetSceneInfo(info);
        }
    }

    public void UpdateSelection(string[] newScenarios, bool addToStack)
    {
        if (addToStack)
            selectionsStack.Push(newScenarios);

        currentlyAvailableSelections.Clear();
        foreach (string key in newScenarios)
        {
            currentlyAvailableSelections.Add(GetSelectionInfo(key));
        }

        GenerateGrid();

        backButton.interactable = selectionsStack.Count > 1;
    }

    public void Back()
    {
        selectionsStack.Pop();
        UpdateSelection(selectionsStack.Peek(), false);
    }

    private VisualizerSelectionInfo GetSelectionInfo(string key)
    {
        foreach (SerializableKeyValuePair<string, VisualizerSelectionInfo> kvp in allSelections)
        {
            if (kvp.Key == key)
            {
                return kvp.Value;
            }
        }
        throw new System.Exception(); // TODO: Custom Exceptions
    }
}
