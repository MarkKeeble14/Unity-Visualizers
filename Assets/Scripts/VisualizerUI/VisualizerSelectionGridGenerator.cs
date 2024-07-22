using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class VisualizerSelectionGridGenerator : MonoBehaviour
{
    [SerializeField] private VisualizerSelectionButton buttonPrefab;
    [SerializeField] private List<SerializableKeyValuePair<string, VisualizerSelectionInfo>> allSelections = new();
    private List<VisualizerSelectionInfo> currentlyAvailableSelections = new();
    [SerializeField] private string[] defaultSelections;
    public static VisualizerSelectionGridGenerator _Instance { get; private set; }

    [SerializeField] private Button backButton;
    [SerializeField] private OverrideHoverControlsComputerScreenControl backButtonScaleTween;

    public Stack<string[]> selectionsStack = new();

    [SerializeField] private float scenarioImagePadding;
    [SerializeField] private float textHolderVerticalPadding;
    [SerializeField] private float buttonFontSize;

    [SerializeField] private float blinkGlitchIntensity = 1;
    [SerializeField] private float blinkGlitchDuration = 0.1f;
    [SerializeField] private Volume volume;

    private bool hasSelectedScene;
    private VisualizerSelectionInfo selectedScene;
    public VisualizerSelectionInfo SelectedScene
    {
        get
        {
            if (hasSelectedScene)
            {
                return selectedScene;
            }
            else
            {
                throw new Exception(); // TODO: Custom Exceptions
            }
        }
    }

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSelection(defaultSelections, true, false);
        GenerateGrid();
    }

    [ContextMenu("SetAesthetics")]
    public void SetAesthetics()
    {
        foreach (Transform t in transform)
        {
            VisualizerSelectionButton b = t.GetComponent<VisualizerSelectionButton>();
            b.SetAesthetics(scenarioImagePadding, textHolderVerticalPadding, buttonFontSize);
        }
    }

    private void GenerateGrid()
    {
        // Clear
        foreach (Transform child in transform)
        {
            ComputerCursor._Instance.RemoveInteractables(child.GetComponents<ComputerScreenControl>());
            Destroy(child.gameObject);
        }

        // Generate
        foreach (VisualizerSelectionInfo info in currentlyAvailableSelections)
        {
            VisualizerSelectionButton spawned = Instantiate(buttonPrefab, transform);
            spawned.SetSceneInfo(info);
            spawned.SetAesthetics(scenarioImagePadding, textHolderVerticalPadding, buttonFontSize);
            ComputerCursor._Instance.AddInteractables(spawned.GetComponents<ComputerScreenControl>());
        }
    }

    public void UpdateSelection(string[] newScenarios, bool addToStack, bool blink)
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
        backButtonScaleTween.Disabled = !backButton.interactable;

        if (blink)
            StartCoroutine(BlinkGlitch());
    }

    private IEnumerator BlinkGlitch()
    {
        SFXManager._Instance.PlayOneShot("Glitch", true, true);

        DigitalGlitchVolume digitalGlitchVolume;
        volume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);

        digitalGlitchVolume.intensity.Override(blinkGlitchIntensity);

        yield return new WaitForSeconds(blinkGlitchDuration);

        digitalGlitchVolume.intensity.Override(0);
    }

    public void Back()
    {
        selectionsStack.Pop();
        UpdateSelection(selectionsStack.Peek(), false, true);
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

    internal void SetSelectedScene(VisualizerSelectionInfo sceneInfo)
    {
        selectedScene = sceneInfo;
        hasSelectedScene = true;
    }
}
