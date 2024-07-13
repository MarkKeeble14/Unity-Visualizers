using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;

[System.Serializable]
public struct FontFileData
{
    [SerializeField] public string FontName;
    [SerializeField] public byte[] FileContents;

    public FontFileData(string fontName, byte[] fileContents)
    {
        FontName = fontName;
        FileContents = fileContents;
    }
}

[System.Serializable]
public struct VisualizerPreset
{
    [SerializeField] public List<FontFileData> Fonts;
    [SerializeField] public List<Color> Colors;
    [SerializeField] public List<Gradient> Gradients;
    [SerializeField] public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> BaseVisualizerElements;
    [SerializeField] public Dictionary<string, VisualizerElementsSettings> VisualizerSpecificElements;
    [SerializeField] public Dictionary<string, float> VisualizerFloatValues;
    [SerializeField] public Dictionary<string, int> VisualizerIntValues;
    [SerializeField] public Dictionary<string, bool> VisualizerBoolValues;

    public VisualizerPreset(List<Color> colors, List<Gradient> gradients, List<FontFileData> fonts, 
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> baseVisualizerElements,
        Dictionary<string, VisualizerElementsSettings> visualizerSpecificElements,
        Dictionary<string, float> floatValues, Dictionary<string, int> intValues, Dictionary<string, bool> boolValues)
    {
        Colors = colors;
        Gradients = gradients;
        Fonts = fonts;
        BaseVisualizerElements = baseVisualizerElements;
        VisualizerSpecificElements = visualizerSpecificElements;
        VisualizerFloatValues = floatValues;
        VisualizerIntValues = intValues;
        VisualizerBoolValues = boolValues;
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;
    }

    public string SavePreset(string label, VisualizerPreset preset)
    {
        // Create folder if neccessary
        string presetsPath = Path.Combine(Application.dataPath, "../Presets");
        if (!Directory.Exists(presetsPath))
        {
            Directory.CreateDirectory(presetsPath);
        }
        string encodedFilePath = Path.Combine(presetsPath, label + ".dat");
        encodedFilePath = encodedFilePath.Replace("/", @"\");

        Debug.Log("Saving Preset (" + label + ") to: " + encodedFilePath);
        File.WriteAllText(encodedFilePath, JsonConvert.SerializeObject(preset, Formatting.None));
        return encodedFilePath;
    }

    public VisualizerPreset LoadPreset(string loadFromPath)
    {
        Debug.Log("Attempting to load data from: " + loadFromPath);
        string json = File.ReadAllText(loadFromPath);

        VisualizerPreset loadedPreset = JsonConvert.DeserializeObject<VisualizerPreset>(json);
        return loadedPreset;
    }
}
