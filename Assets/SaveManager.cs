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
    [SerializeField] public List<Color> Colors;
    [SerializeField] public List<Gradient> Gradients;
    [SerializeField] public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> VisualizerElements;
    [SerializeField] public List<FontFileData> Fonts;

    public VisualizerPreset(List<Color> colors, List<Gradient> gradients, List<FontFileData> fonts, Dictionary<VisualizerElementLabel, VisualizerElementsSettings> visualizerElements)
    {
        Colors = colors;
        Gradients = gradients;
        Fonts = fonts;
        VisualizerElements = visualizerElements;
    }

    public override string ToString()
    {
        string s = "Colors: " + Colors.Count;
        Colors.ForEach(x => { s += ", " + x; });
        s += " - Gradients: " + Gradients.Count;
        s += " - Visualizer Elements";
        foreach (VisualizerElementLabel item in VisualizerElements.Keys)
        {
            s += "," + item + " Enabled?: " + VisualizerElements[item].Enabled + ", Color: " 
                + VisualizerElements[item].ColorIndex + ", Font: " 
                + VisualizerElements[item].FontIndex;
        }
        s += " - Fonts: " + Fonts.Count;
        return s;
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
