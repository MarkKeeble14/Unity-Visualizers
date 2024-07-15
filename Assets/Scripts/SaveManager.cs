using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
    [SerializeField] public string Title;
    [SerializeField] public SpriteData CoverArt;
    [SerializeField] public AudioClipData Audio;

    public VisualizerPreset(string title, SpriteData coverArt, AudioClipData audio,
        List<Color> colors, List<Gradient> gradients, List<FontFileData> fonts, 
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> baseVisualizerElements,
        Dictionary<string, VisualizerElementsSettings> visualizerSpecificElements,
        Dictionary<string, float> floatValues, Dictionary<string, int> intValues, Dictionary<string, bool> boolValues)
    {
        Title = title;
        CoverArt = coverArt;
        Audio = audio;
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

    public async ValueTask<string> SavePreset(string label, VisualizerPreset preset)
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

        int loadingKey = UIManager._Instance.AddLoading("Saving Preset...");

        var task = Task.Run(async () => await File.WriteAllTextAsync(encodedFilePath, JsonConvert.SerializeObject(preset, Formatting.None)));
        await task;

        UIManager._Instance.RemoveLoading(loadingKey);
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Preset saved to " + encodedFilePath);

        return encodedFilePath;
    }

    public async Task LoadPreset(string loadFromPath, Action<string, VisualizerPreset> onSuccess, Action<string> onFailure)
    {
        Debug.Log("Attempting to load data from: " + loadFromPath);
        int loadingKey = UIManager._Instance.AddLoading("Loading Preset...");

        try
        {
            var readFileTask = Task.Run(async () => await File.ReadAllTextAsync(loadFromPath));
            await readFileTask;

            string json = readFileTask.Result;
            VisualizerPreset loadedPreset = new();

            var deserializeTask = await Task.Run(() => loadedPreset = JsonConvert.DeserializeObject<VisualizerPreset>(json));

            onSuccess?.Invoke(loadFromPath, loadedPreset);
        } catch (Exception e)
        {
            Debug.LogError(e);
            onFailure?.Invoke(loadFromPath);
        }

        UIManager._Instance.RemoveLoading(loadingKey);
    }
}
