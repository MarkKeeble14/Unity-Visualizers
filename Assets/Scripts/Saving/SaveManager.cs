using Assets.SimpleZip;
using FFMpegCore.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct GradientData
{
    [SerializeField] public GradientColorKey[] ColorKeys;
    [SerializeField] public GradientAlphaKey[] AlphaKeys;
    [SerializeField] public GradientMode Mode;

    public GradientData(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys, GradientMode mode)
    {
        ColorKeys = colorKeys;
        AlphaKeys = alphaKeys;
        Mode = mode;
    }
}

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
    [SerializeField] public List<GradientData> Gradients;
    [SerializeField] public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> BaseVisualizerElements;
    [SerializeField] public Dictionary<string, float> VisualizerFloatValues;
    [SerializeField] public Dictionary<string, int> VisualizerIntValues;
    [SerializeField] public Dictionary<string, bool> VisualizerBoolValues;
    [SerializeField] public string Title;
    [SerializeField] public AudioClipData Audio;
    [SerializeField] public SpriteData CoverArt;
    [SerializeField] public SpriteData Background;

    public VisualizerPreset(string title, AudioClipData audio, SpriteData coverArt, SpriteData background,
        List<Color> colors, List<Gradient> gradients, List<FontFileData> fonts, 
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> baseVisualizerElements,
        Dictionary<string, float> floatValues, Dictionary<string, int> intValues, Dictionary<string, bool> boolValues)
    {
        Title = title;
        Audio = audio;
        CoverArt = coverArt;
        Background = background;
        Colors = colors;

        Gradients = new List<GradientData>();
        foreach (Gradient g in gradients)
        {
            Gradients.Add(new GradientData(g.colorKeys, g.alphaKeys, g.mode));
        }

        Fonts = fonts;
        BaseVisualizerElements = baseVisualizerElements;
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

    public string PresetsPath { get { return Path.Combine(Application.dataPath, "../Presets"); } }

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
            if (!System.IO.Directory.Exists(PresetsPath))
            {
                System.IO.Directory.CreateDirectory(PresetsPath);
                Debug.Log("Created directory at: " + PresetsPath);
            }

            encodedFilePath = Path.Combine(PresetsPath, label + ".dat");
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

        string niceFilePath = Path.GetFullPath(encodedFilePath);

        int loadingKey1 = UIManager._Instance.AddLoading("Saving Preset to " + Path.GetFileName(niceFilePath));

        int loadingKey2 = UIManager._Instance.AddLoading("Serializing Preset...");

        string json = "";
        var serializeTask = Task.Run(() => { json = JsonConvert.SerializeObject(preset, Formatting.None); });
        await serializeTask;

        UIManager._Instance.RemoveLoading(loadingKey2);

        loadingKey2 = UIManager._Instance.AddLoading("Compressing Preset...");

        byte[] jsonAsBytes = Encoding.ASCII.GetBytes(json);
        byte[] array = new byte[0];
        var compressionTask = Task.Run(() => { array = Zip.Compress(jsonAsBytes); });
        await compressionTask;

        UIManager._Instance.RemoveLoading(loadingKey2);

        loadingKey2 = UIManager._Instance.AddLoading("Writing to File...");

        var writeTask = Task.Run(async () => await System.IO.File.WriteAllBytesAsync(encodedFilePath, array));
        await writeTask;

        UIManager._Instance.RemoveLoading(loadingKey2);

        UIManager._Instance.RemoveLoading(loadingKey1);
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Preset saved to " + niceFilePath);

        onSuccess?.Invoke(encodedFilePath);

        return true;
    }

    public async Task LoadPreset(string loadFromPath, bool isDefaultPreset, Action<string, VisualizerPreset> onSuccess, Action<string> onFailure)
    {
        string fileName = StringHelper.GetFileName(loadFromPath);
        if (!File.Exists(loadFromPath))
        {
            Debug.Log("File could not be found at: " + loadFromPath);

            if (isDefaultPreset)
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Could not locate default preset - " +
                    "was a file deleted from the StreamingAssets folder?");
            } else
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Could not locate requested file");
            }

                onFailure?.Invoke(loadFromPath);
            return;
        }

        Debug.Log("Loading data from: " + loadFromPath);
        string fileExtension = StringHelper.GetFileExtension(loadFromPath);
        int loadingKey = UIManager._Instance.AddLoading("Loading Preset=" 
            + StringHelper.GetFileName(loadFromPath) + "." + fileExtension, 3);

        if (!fileExtension.Equals("dat"))
        {
            Debug.Log("Incorrect file type specified for loading preset");
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, "Selected file is incorrect " +
                "- Preset files will have the .dat file extension");

            UIManager._Instance.RemoveLoading(loadingKey);
            onFailure?.Invoke(loadFromPath);
            return;
        }

        VisualizerPreset loadedPreset = new();
        try
        {
            int loadingKey2 = UIManager._Instance.AddLoading("Reading File...");

            Task<byte[]> readFileTask;
            try
            {
                readFileTask = Task.Run(async () => await System.IO.File.ReadAllBytesAsync(loadFromPath));
                await readFileTask;
            } catch (Exception e)
            {
                Debug.Log("Unable to read file");
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, "Unable to parse file " +
                    "- File is either not a preset file or contains corrupted data");
                UIManager._Instance.RemoveLoading(loadingKey2);
                throw new AllPartOfThePlanException();
            }

            UIManager._Instance.RemoveLoading(loadingKey2);

            loadingKey2 = UIManager._Instance.AddLoading("Decompressing...");

            byte[] readBytes = readFileTask.Result;
            byte[] jsonAsBytes = new byte[0];

            try
            {
                var decompressTask = await Task.Run(() => jsonAsBytes = Zip.Decompress(readBytes));
            } catch (Exception e)
            {
                Debug.Log("Unable to decompress file");
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, "Unable to decompress file " +
                    "- File may be corrupted");
                UIManager._Instance.RemoveLoading(loadingKey2);
                throw new AllPartOfThePlanException();
            }

            UIManager._Instance.RemoveLoading(loadingKey2);

            string json = Encoding.ASCII.GetString(jsonAsBytes);

            loadingKey2 = UIManager._Instance.AddLoading("Deserializing...");

            try
            {
                var deserializeTask = await Task.Run(() => loadedPreset = JsonConvert.DeserializeObject<VisualizerPreset>(json));
            } catch (Exception e)
            {
                Debug.Log("Unable to desieralize json");
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, "Unable to desieralize json " +
                    "- File may be corrupted");
                UIManager._Instance.RemoveLoading(loadingKey2);
                throw new AllPartOfThePlanException();
            }

            UIManager._Instance.RemoveLoading(loadingKey2);
        } catch (Exception e)
        {
            Debug.LogError(e);
            onFailure?.Invoke(loadFromPath);
        }

        UIManager._Instance.RemoveLoading(loadingKey);

        onSuccess?.Invoke(loadFromPath, loadedPreset);
    }
}
