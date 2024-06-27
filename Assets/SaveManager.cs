using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct VisualizerPreset : ISerializationCallbackReceiver
{
    [SerializeField] public List<Color> Colors;
    [SerializeField] public List<Gradient> Gradients;
    // [SerializeField] public List<TMP_FontAsset> Fonts;

    public VisualizerPreset(List<Color> colors, List<Gradient> gradients)
    {
        Colors = colors;
        Gradients = gradients;
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
    }

    private void Print()
    {
        Debug.Log("Colors: " + Colors.Count + ", Gradients: " + Gradients.Count);
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

    public void SavePreset(string label, VisualizerPreset preset)
    {
        // Create folders if neccessary
        string presetsPath = Path.Combine(Application.dataPath, "../Presets");
        if (!Directory.Exists(presetsPath))
        {
            Directory.CreateDirectory(presetsPath);
        }

        string encodedFilePath = Path.Combine(presetsPath, label + ".dat");
        encodedFilePath = encodedFilePath.Replace("/", @"\");

        Debug.Log("Saving Preset (" + label + ") to: " + encodedFilePath);
        File.WriteAllText(encodedFilePath, JsonConvert.SerializeObject(preset, Formatting.Indented));
    }

    public VisualizerPreset LoadPreset(string loadFromPath)
    {
        Debug.Log("Attempting to load data from: " + loadFromPath);
        string json = File.ReadAllText(loadFromPath);

        VisualizerPreset loadedPreset = JsonConvert.DeserializeObject<VisualizerPreset>(json);
        return loadedPreset;
    }
}
