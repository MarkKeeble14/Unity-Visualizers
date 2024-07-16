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
    [SerializeField] public AudioClipData Audio;
    [SerializeField] public SpriteData CoverArt;

    public VisualizerPreset(string title, AudioClipData audio, SpriteData coverArt,
        List<Color> colors, List<Gradient> gradients, List<FontFileData> fonts, 
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> baseVisualizerElements,
        Dictionary<string, VisualizerElementsSettings> visualizerSpecificElements,
        Dictionary<string, float> floatValues, Dictionary<string, int> intValues, Dictionary<string, bool> boolValues)
    {
        Title = title;
        Audio = audio;
        CoverArt = coverArt;
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

    public async ValueTask<bool> SavePreset(string label, VisualizerPreset preset, Action<string> onSuccess, Action onFailure)
    {
        if (label.Contains('/') || label.Contains('\\'))
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Preset name contains either a forward slash (/) or back slash (\\) " +
                " - This is not allowed");

            onFailure?.Invoke();
            return false;
        }

        // Create folder if neccessary
        string encodedFilePath;
        try
        {
            string presetsPath = Path.Combine(Application.dataPath, "../Presets");

            if (!Directory.Exists(presetsPath))
            {
                Directory.CreateDirectory(presetsPath);
                Debug.Log("Created directory at: " + presetsPath);
            }

            encodedFilePath = Path.Combine(presetsPath, label + ".dat");
        }
        catch (Exception e)
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Preset name could not be used for file name " +
                "- Ensure no characters used are illegal to use in file names");
            onFailure?.Invoke();
            Debug.LogError(e);

            return false;
        }

        Debug.Log("Saving Preset (" + label + ") to: " + encodedFilePath);

        int loadingKey1 = UIManager._Instance.AddLoading("Saving Preset...");

        int loadingKey2 = UIManager._Instance.AddLoading("Serializing Preset...");

        string json = "";
        var serializeTask = Task.Run(() => { json = JsonConvert.SerializeObject(preset, Formatting.None); });
        await serializeTask;

        UIManager._Instance.RemoveLoading(loadingKey2);

        loadingKey2 = UIManager._Instance.AddLoading("Writing to File...");

        var writeTask = Task.Run(async () => await File.WriteAllTextAsync(encodedFilePath, json));
        await writeTask;

        File.WriteAllText(encodedFilePath, json);

        UIManager._Instance.RemoveLoading(loadingKey2);

        UIManager._Instance.RemoveLoading(loadingKey1);
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Preset saved to " + encodedFilePath);

        onSuccess?.Invoke(encodedFilePath);

        return true;
    }

    public async Task LoadPreset(string loadFromPath, Action<string, VisualizerPreset> onSuccess, Action<string> onFailure)
    {
        Debug.Log("Attempting to load data from: " + loadFromPath);
        int loadingKey = UIManager._Instance.AddLoading("Loading Preset...");

        try
        {
            int loadingKey2 = UIManager._Instance.AddLoading("Reading File...");

            var readFileTask = Task.Run(async () => await File.ReadAllTextAsync(loadFromPath));
            await readFileTask;

            string json = readFileTask.Result;

            UIManager._Instance.RemoveLoading(loadingKey2);

            loadingKey2 = UIManager._Instance.AddLoading("Deserializing...");

            VisualizerPreset loadedPreset = new();
            var deserializeTask = await Task.Run(() => loadedPreset = JsonConvert.DeserializeObject<VisualizerPreset>(json));

            UIManager._Instance.RemoveLoading(loadingKey2);

            onSuccess?.Invoke(loadFromPath, loadedPreset);
        } catch (Exception e)
        {
            Debug.LogError(e);
            onFailure?.Invoke(loadFromPath);
        }

        UIManager._Instance.RemoveLoading(loadingKey);
    }
}
