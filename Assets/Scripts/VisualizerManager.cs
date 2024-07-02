using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Device;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public enum AudioChannel
{
    STEREO,
    LEFT,
    RIGHT
}

[System.Serializable]
public struct VisualizerElementsSettings
{
    [SerializeField, HideInInspector] public int ColorIndex;
    [SerializeField, HideInInspector] public int FontIndex;
    [SerializeField, HideInInspector] public bool Enabled;

    public VisualizerElementsSettings(int colorIndex, int fontIndex, bool enabled)
    {
        ColorIndex = colorIndex;
        FontIndex = fontIndex;
        Enabled = enabled;
    }
}

[RequireComponent(typeof(AudioSource))]
public class VisualizerManager : MonoBehaviour
{
    public static VisualizerManager _Instance { get; private set; }

    [Header("Default Track Info")]
    [SerializeField] private TrackInfo trackInfo;
    [SerializeField] private TMP_FontAsset defaultFont;

    private Dictionary<string, int> loadedFontIndices = new();
    private Dictionary<string, FontFileData> loadedFontData = new();
    private Dictionary<string, TMP_FontAsset> loadedTMPFontAssets = new();

    [Header("Other Settings")]
    [SerializeField] private bool askForTrack = true;
    [SerializeField] private bool askForCoverArt = true;
    [SerializeField] private bool askForPreset = true;
    [SerializeField] private bool askForColors = true;
    [SerializeField] private bool askForFonts = true;
    [SerializeField] private bool askForVisualizerElements = true;
    [SerializeField] private bool startImmedietelyUponLoadingTrack;
    [SerializeField] private float startSongAtSeconds;
    [SerializeField] private bool applyPostProcessing;
    [SerializeField, Range(0, 1)] private float volumeWeight;


    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private float audioSampleSmoothing = 100;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    [Header("Recording Settings")]
    [SerializeField] private float afterTrackRecordingBufferTime = 10f;

    [Header("References")]
    [SerializeField] private CanvasGroup visualizerCanvasGroup;
    [SerializeField] private GameObject visualizerElementsUI;
    private AudioSource audioSource;
    private UniversalAdditionalCameraData activeCameraAdditionalCameraData;
    private Volume volume;

    [Header("Scenarios")]
    [SerializeField] private List<SerializableKeyValuePair<string, GameObject>> scenarios = new();
    [SerializeField] private GameObject defaultScenary;
    [SerializeField] private GameObject scenarioSelection;

    [Header("Transition Settings")]
    [SerializeField] private TransitionData initialTransition;

    [Header("Colors")]
    [SerializeField] private GameObject colorsUI;
    [SerializeField] private Transform colorsList;
    [SerializeField] private Transform fontsList;
    [SerializeField] private GameObject fontsUI;

    [Header("Prefabs")]
    [SerializeField] private ColorListElement colorListElement;
    [SerializeField] private GradientListElement gradientListElement;
    [SerializeField] private FontListElement fontListElement;

    private List<IRecieveTrackInfo> trackInfoListeners = new();
    private List<IRecieveVisualizerElementsInfo> visualizerElements = new();
    private Dictionary<VisualizerElementLabel, VisualizerElementsSettings> visualizerElementsInfo = new();

    private bool hasSongStarted = false;
    private float lastAudioSourceTime;

    // Events
    public Action OnSongEnd;
    public Action OnSongStart;

    private float[] leftAudioSamples = new float[512];
    private float[] rightAudioSamples = new float[512];

    // Audio Data
    private float[] frequencyBands = new float[8];
    private float[] frequencyBandBuffer = new float[8];
    private float[] frequencyBandBufferDecrease = new float[8];
    private float[] highestValuePerFrequencyBand = new float[8];
    private float[] audioBands = new float[8];
    private float[] audioBandsBuffer = new float[8];

    private float amplitude;
    private float amplitudeBuffer;
    private float highestAmplitude;

    // Properties
    public float HighestAmplitude => highestAmplitude;
    public float AverageAmplitude { get { return amplitude / highestAmplitude; } }
    public float AverageAmplitudeBuffer { get { return amplitudeBuffer / AverageAmplitude; } }
    public float[] AudioSamples => leftAudioSamples;
    public float ZeroSample { get; private set; }

    public float PlaythroughPercent
    {
        get
        {
            if (audioSource.clip == null) return 0;
            return audioSource.time / audioSource.clip.length;
        }
    }

    public string SecondsPlayed
    {
        get
        {
            return StringHelper.GetDurationText(audioSource.time);
        }
    }

    public float PlaybackTime
    {
        get
        {
            return audioSource.time;
        }
    }

    public bool IsPlaybackPaused => !audioSource.isPlaying && audioSource.time > 0;

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }

    private List<IRecieveActiveCamera> activeCameraListeners = new();

    [ContextMenu("BroadcastActiveCamera")]
    private void BroadcastActiveCamera()
    {
        activeCameraListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveActiveCamera>().ToList();
        
        // Send data out
        activeCameraListeners.ForEach(item => item.RecieveActiveCamera(Camera.main));
    }


    [ContextMenu("BroadcastTrackInfo")]
    private void BroadcastTrackInfo()
    {
        trackInfoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTrackInfo>().ToList();

        if (audioSource.clip == null) return;

        UpdateTrackFonts();

        // Send data out
        trackInfoListeners.ForEach(item => item.RecieveTrackInfo(trackInfo));
    }

    [ContextMenu("BroadcastVisualizerElementsInfo")]
    private void BroadcastVisualizerElementsInfo()
    {
        // Attempt to find any listeners if the list is empty
        // I don't foresee visualizer elements being spawned as the game progresses, so caching them once at the beggining should be fine
        if (visualizerElements.Count == 0)
        {
            visualizerElements = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerElementsInfo>().ToList();
        }

        // Send data out
        visualizerElements.ForEach(item => item.RecieveVisualizerElementsInfo(visualizerElementsInfo));
    }

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        // Get audio source component
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = trackInfo.AudioClip;
        trackInfo.Duration = StringHelper.GetDurationText(trackInfo.AudioClip.length);
        volume = FindObjectOfType<Volume>();
    }

    private void Start()
    {
        PopulateColorsList();
        PopulateFontsList();

        // Populate visualizer elements info
        foreach (VisualizerElementLabel item in Enum.GetValues(typeof(VisualizerElementLabel)))
        {
            visualizerElementsInfo.Add(item, new VisualizerElementsSettings(0, 0, true));
        }

        // Only 1 scenario, we'd just go to track selection
        if (scenarios.Count == 0)
        {
            StartCoroutine(RunSetup());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Enable/disable post processing
        if (activeCameraAdditionalCameraData != null)
            activeCameraAdditionalCameraData.renderPostProcessing = applyPostProcessing;

        // Change weight
        volume.weight = volumeWeight;

        // Spectrum Data
        GetSpectrumAudioSource();

        if (leftAudioSamples != null && leftAudioSamples.Length > 0)
        {
            ZeroSample = leftAudioSamples[0] * audioSampleSmoothing;
        }

        // Make Frequency Bands
        MakeFrequencyBands();

        // Band Buffer
        CalcBandBuffer();

        // Create Audio Bands
        CreateAudioBands();

        GetAmplitude();

        // Determine if Song has Started/Ended
        if (!hasSongStarted && audioSource.time != lastAudioSourceTime)
        {
            hasSongStarted = true;
            OnSongStart?.Invoke();
        }

        if (hasSongStarted && audioSource.time == 0)
        {
            hasSongStarted = false;
            OnSongEnd?.Invoke();
        }

        lastAudioSourceTime = audioSource.time;
    }

    private void SetupComplete()
    {
        // Set canvas group alpha
        visualizerCanvasGroup.alpha = 1;
        visualizerCanvasGroup.blocksRaycasts = true;

        // Create audio profile
        CreateAudioProfile();

        // Inform listeners of track
        BroadcastTrackInfo();

        // Play initial transition if there is one
        if (initialTransition.transition != null)
            initialTransition.transition.InitiateTransition(initialTransition.direction);

        if (startImmedietelyUponLoadingTrack)
        {
            BeginPlayback();
        }
    }

    public void BeginPlayback()
    {
        if (startSongAtSeconds > audioSource.clip.length)
            Debug.LogWarning("Attempted to start the track at a position longer than the track itself");

        // Set the point where the AudioSource begins
        audioSource.time = startSongAtSeconds;

        // Set the max duration of time a recording can go on for
        ScreenRecorder._Instance.MaxRecordingTime = audioSource.clip.length + afterTrackRecordingBufferTime;

        // Play the Track
        audioSource.Play();
    }

    public Color GetColor(VisualizerColorType type, int index)
    {
        switch (type)
        {
            case VisualizerColorType.COLOR:
                return GetTrackColor(index);
            case VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(0);
            case VisualizerColorType.TIME_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(PlaythroughPercent);
            default:
                throw new Exception(); // TODO: Custom Exceptions
        }
    }

    public Color GetColor(VisualizerColorType type, int index, float f)
    {
        switch (type)
        {
            case VisualizerColorType.COLOR:
                return GetTrackColor(index);
            case VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(f);
            case VisualizerColorType.TIME_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(PlaythroughPercent);
            default:
                throw new Exception(); // TODO: Custom Exceptions
        }
    }

    public Color GetTrackColor(int index) 
    {
        if (index > trackInfo.Colors.Count - 1) return trackInfo.Colors[0];
        return trackInfo.Colors[index]; 
    
    }

    public Gradient GetTrackGradient(int index) 
    {
        if (trackInfo.Gradients.Count - 1 == 0) return trackInfo.Gradients[0];
        return trackInfo.Gradients[index]; 
    }

    private string GetFontKeyAtIndex(int index)
    {
        foreach (KeyValuePair<string, int> kvp in loadedFontIndices)
        {
            if (kvp.Value == index)
            {
                return kvp.Key;
            }
        }
        throw new IndexOutOfRangeException();
    }

    public TMP_FontAsset GetFont(int index)
    {
        if (index > loadedFontData.Count - 1) return defaultFont;
        return loadedTMPFontAssets[GetFontKeyAtIndex(index)];
    }

    public TMP_FontAsset GetDefaultFont()
    {
        return defaultFont;
    }

    public string GetFontName(int index)
    {
        return loadedFontData[GetFontKeyAtIndex(index)].FontName;
    }

    private void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);
    }

    private void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                switch (channel)
                {
                    case AudioChannel.STEREO:
                        average += (leftAudioSamples[count] + rightAudioSamples[count]) * (count + 1);
                        break;
                    case AudioChannel.LEFT:
                        average += leftAudioSamples[count] * (count + 1);
                        break;
                    case AudioChannel.RIGHT:
                        average += rightAudioSamples[count] * (count + 1);
                        break;
                }
                count++;
            }

            average /= count;

            frequencyBands[i] = average * 10;
        }
    }

    private void CreateAudioBands()
    {
        for (int i = 0; i < audioBands.Length; i++)
        {
            if (frequencyBands[i] > highestValuePerFrequencyBand[i])
            {
                highestValuePerFrequencyBand[i] = frequencyBands[i];
            }
            audioBands[i] = frequencyBands[i] / highestValuePerFrequencyBand[i];
            audioBandsBuffer[i] = frequencyBandBuffer[i] / highestValuePerFrequencyBand[i];
        }
    }

    private void CalcBandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (frequencyBands[g] > frequencyBandBuffer[g])
            {
                frequencyBandBuffer[g] = frequencyBands[g];
                frequencyBandBufferDecrease[g] = defaultBandBufferDecrease;
            }

            if (frequencyBands[g] < frequencyBandBuffer[g])
            {
                frequencyBandBuffer[g] -= frequencyBandBufferDecrease[g];
                frequencyBandBufferDecrease[g] *= bandBufferDecreaseMultPerFrame;
            }
        }
    }

    private void GetAmplitude()
    {
        amplitude = 0;
        amplitudeBuffer = 0;
        for (int i = 0; i < frequencyBands.Length; i++)
        {
            amplitude += frequencyBands[i];
            amplitudeBuffer += frequencyBandBuffer[i];
        }
        if (amplitude > highestAmplitude) highestAmplitude = amplitude;
    }

    private void CreateAudioProfile()
    {
        for (int i = 0; i < highestValuePerFrequencyBand.Length; i++)
            highestValuePerFrequencyBand[i] = beginningHighestFrequencyBandValue;
    }

    public void PausePlayback()
    {
        audioSource.Pause();
    }

    public void ResumePlayback()
    {
        audioSource.UnPause();
    }

    public void ResetPlayback()
    {
        hasSongStarted = false;
        audioSource.time = 0;
        PausePlayback();
    }

    public void Seek(float amount)
    {
        audioSource.time += amount;
    }

    public void SelectScenario(string name)
    {
        defaultScenary.SetActive(false);
        scenarioSelection.SetActive(false); ;

        foreach (SerializableKeyValuePair<string, GameObject> kvp in scenarios)
        {
            kvp.Value.SetActive(kvp.Key == name);
        }

        BroadcastActiveCamera();

        activeCameraAdditionalCameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();

        StartCoroutine(RunSetup());
    }

    private IEnumerator RunSetup()
    {
        if (askForTrack)
            yield return StartCoroutine(RunTrackSelection());

        if (askForCoverArt)
            yield return StartCoroutine(RunCoverArtSelection());

        if (askForPreset)
            yield return StartCoroutine(RunLoadPresetSelection());

        if (askForColors)
            yield return StartCoroutine(RunColorsSelection());

        if (askForFonts)
            yield return StartCoroutine(RunFontsSelection());

        if (askForVisualizerElements)
            yield return StartCoroutine(RunVisualizerElementsSelection());

        SetupComplete();
    }

    public IEnumerator RunFontsSelection()
    {
        fontsUI.SetActive(true);

        yield return new WaitUntil(() => !fontsUI.activeInHierarchy);

        BroadcastTrackInfo();
    }

    public IEnumerator RunTrackSelection()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Audio from File: " + x);

            StartCoroutine(LoadAudioClipFromFile(x,
                (filePath, clip) =>
                {
                    Debug.Log("Successfully Loaded Audio Clip from path = " + x);

                    audioSource.clip = clip;
                    trackInfo.Title = StringHelper.GetFileName(filePath);
                    trackInfo.Duration = StringHelper.GetDurationText(trackInfo.AudioClip.length);

                    BroadcastTrackInfo();
                },
                x => Debug.Log("Failed to Load Audio Clip from path = " + x)));
        }, "Select Track", "Load"));
    }

    public IEnumerator RunCoverArtSelection()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpeg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Image from File: " + x);
            LoadImageFromFile(x,
                (filePath, texture) =>
                {
                    Debug.Log("Successfully Loaded Image from path = " + x);
                    trackInfo.CoverArt = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    BroadcastTrackInfo();
                },
            x => Debug.Log("Failed to Load Image from path = " + x));
        }, "Select Cover Art", "Load"));
    }

    public IEnumerator RunColorsSelection()
    {
        // Enable UI
        colorsUI.SetActive(true);

        yield return new WaitUntil(() => !colorsUI.activeSelf);

        BroadcastTrackInfo();
    }

    public IEnumerator RunVisualizerElementsSelection()
    {
        // Enable UI
        visualizerElementsUI.SetActive(true);

        BroadcastVisualizerElementsInfo();

        yield return new WaitUntil(() => !visualizerElementsUI.activeSelf);

        BroadcastVisualizerElementsInfo();
    }

    public IEnumerator RunLoadPresetSelection()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Presets", ".dat"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Preset from File: " + x);
            LoadPreset(x,
                (filePath, visualizerPreset) =>
                {
                    Debug.Log("Successfully Loaded Preset from path = " + x);
                },
            x => Debug.Log("Failed to Load Preset from path = " + x));
        }, "Select Preset", "Load"));

        BroadcastTrackInfo();
    }

    public IEnumerator SelectOneFont(Action<string, TMP_FontAsset> onSuccess)
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Font", ".ttf", ".otf"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            TMP_FontAsset font = LoadTMPFontFromFile(x);
            onSuccess(x, font);
        }, "Select Font", "Load"));
    }

    private IEnumerator BrowseForSingleFile(Action<string> toDoWithFile, string dialogTitle, string loadButtonText)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders,
            false, null, null, dialogTitle, loadButtonText);

        if (FileBrowser.Success)
            OnFileSucessfullySelected(FileBrowser.Result[0], toDoWithFile);
        else
            OnFailureToSelectFiles();
    }

    private IEnumerator BrowseForMultipleFiles(Action<string> toDoWithFile, string dialogTitle, string loadButtonText)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders,
            true, null, null, dialogTitle, loadButtonText);

        if (FileBrowser.Success)
        {
            foreach (string filePath in FileBrowser.Result)
            {
                OnFileSucessfullySelected(filePath, toDoWithFile);
            }
        }
        else
        {
            OnFailureToSelectFiles();
        }
    }

    private void OnFileSucessfullySelected(string filePath, Action<string> toDoWithFile)
    {
        toDoWithFile(filePath);
    }

    private void OnFailureToSelectFiles()
    {
        Debug.LogWarning("Cancelled File Selection");
    }

    private IEnumerator LoadAudioClipFromFile(string filePath, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, GetAudioType(filePath)))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
                onFailure(filePath);
            }
            else
            {
                onSuccess(filePath, DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }

    private void LoadImageFromFile(string filePath, Action<string, Texture2D> onSuccess, Action<string> onFailure)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        if (tex.LoadImage(bytes))
        {
            onSuccess(filePath, tex);
        } else
        {
            onFailure(filePath);
        }
    }

    private TMP_FontAsset LoadTMPFontFromFile(string filePath)
    {
        Debug.Log("Loading Font from File: " + filePath);
        return LoadTMPFontFromFontAsset(new Font(filePath));
    }

    private TMP_FontAsset LoadTMPFontFromFontAsset(Font font)
    {
        TMP_FontAsset asset = TMP_FontAsset.CreateFontAsset(font);
        return asset;
    }

    private Font LoadFontFromByteArray(byte[] bytes)
    {
        string filePath = UnityEngine.Application.dataPath + "/LoadingFont.ttf";
        Debug.Log("Loading font from byte array - Helper file at: " + filePath);
        File.WriteAllBytes(filePath, bytes);
        return new Font(filePath);
    }


    private AudioType GetAudioType(string filePath)
    {
        switch (StringHelper.GetFileExtension(filePath))
        {
            case "wav":
                return AudioType.WAV;
            case "mp3":
                return AudioType.MPEG;
            case "ogg":
                return AudioType.OGGVORBIS;
            default:
                throw new Exception(); // TODO: Custom Exceptions
        }
    }

    public void AddElementToColorsList()
    {
        UIManager._Instance.PopupActionSelection("Color or Gradient?", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("Color", () => AddColorElement()),
            new ActionSelection("Gradient", () => AddGradientElement()),
        });

    }

    private void AddColorElement()
    {
        trackInfo.Colors.Add(Color.white);
        ColorListElement spawned = Instantiate(colorListElement, colorsList);
        spawned.Set(trackInfo.Colors.Count - 1, Color.white);
    }

    private void AddGradientElement()
    {
        Gradient g = new Gradient();
        trackInfo.Gradients.Add(g);
        GradientListElement spawned = Instantiate(gradientListElement, colorsList);
        spawned.Set(trackInfo.Gradients.Count - 1, g);
    }

    private void PopulateColorsList()
    {
        for (int i = 0; i < trackInfo.Colors.Count; ++i)
        {
            ColorListElement spawned = Instantiate(colorListElement, colorsList);
            spawned.Set(i, trackInfo.Colors[i]);
        }

        for (int i = 0; i < trackInfo.Gradients.Count; ++i)
        {
            GradientListElement spawned = Instantiate(gradientListElement, colorsList);
            spawned.Set(i, trackInfo.Gradients[i]);
        }
    }

    private void ClearColorsList()
    {
        foreach (Transform child in colorsList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateTrackColor(int index, Color c)
    {
        if (index > trackInfo.Colors.Count - 1) return;
        trackInfo.Colors[index] = c;
        BroadcastTrackInfo();
    }

    public void UpdateTrackGradient(int index, Gradient g)
    {
        if (index > trackInfo.Gradients.Count - 1) return;
        trackInfo.Gradients[index] = g;
        BroadcastTrackInfo();
    }

    private void ClearFontsList()
    {
        foreach (Transform child in fontsList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddFontElement()
    {
        StartCoroutine(BrowseForMultipleFiles(x =>
        {
            string fontName = RegisterNewFont(x);
            FontListElement spawned = Instantiate(fontListElement, fontsList);
            spawned.Set(loadedFontIndices[fontName]);
        }, "Choose Fonts", "Load"));
    }

    private void PopulateFontsList()
    {
        for (int i = 0; i < loadedFontData.Count; ++i)
        {
            FontListElement spawned = Instantiate(fontListElement, fontsList);
            spawned.Set(i);
        }
    }

    public void UpdateFont(string filePath, int index)
    {
        string newFontName = StringHelper.GetFileName(filePath);
        if (loadedFontData.ContainsKey(newFontName))
        {
            Debug.LogWarning("Ignoring attempt to add a font that already exists");
            return;
        }

        // Remove old
        string keyAtIndex = GetFontKeyAtIndex(index);
        loadedFontIndices.Remove(keyAtIndex);
        loadedFontData.Remove(keyAtIndex);
        loadedTMPFontAssets.Remove(keyAtIndex);

        // Add new
        RegisterNewFont(filePath, index);

        BroadcastTrackInfo();
    }

    public string RegisterNewFont(string filePath, int index = -1)
    {
        byte[] fileContents = File.ReadAllBytes(filePath);
        FontFileData fontFileData = new FontFileData(StringHelper.GetFileName(filePath), fileContents);

        loadedFontData.Add(fontFileData.FontName, fontFileData);
        loadedTMPFontAssets.Add(fontFileData.FontName, LoadTMPFontFromFontAsset(LoadFontFromByteArray(fileContents)));
        loadedFontIndices.Add(fontFileData.FontName, (index == -1 ? loadedFontIndices.Count : index));
        return fontFileData.FontName;
    }

    private void UpdateTrackFonts()
    {
        trackInfo.Fonts.Clear();

        string[] keys = loadedFontIndices.Keys.ToArray();
        int findingElementAtIndex = 0;

        while (findingElementAtIndex < keys.Length)
        {
            for (int i = 0; i < keys.Count(); ++i)
            {
                string key = keys[i];
                int value = loadedFontIndices[key];

                if (value == findingElementAtIndex)
                {
                    trackInfo.Fonts.Add(loadedTMPFontAssets[key]);
                    findingElementAtIndex++;
                }
            }
        }
    }

    public VisualizerElementsSettings GetVisualizerElementSettings(VisualizerElementLabel label)
    {
        return visualizerElementsInfo[label];
    }

    public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> GetVisualizerElementSettings()
    {
        return visualizerElementsInfo;
    }

    public void SetVisualizerElementsSettings(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> settings)
    {
        visualizerElementsInfo = settings;
        BroadcastVisualizerElementsInfo();
    }

    public void UpdateVisualizerElementSettings(VisualizerElementLabel label, VisualizerElementsSettings newSettings)
    {
        visualizerElementsInfo[label] = newSettings;
        BroadcastVisualizerElementsInfo();
    }

    public void ChangeVolume(float amount)
    {
        audioSource.volume += amount;
    }

    [ContextMenu("Save Preset")]
    public void SavePreset()
    {
        VisualizerPreset preset = new VisualizerPreset(trackInfo.Colors, trackInfo.Gradients,
            loadedFontData.Values.ToList(), visualizerElementsInfo);

        UIManager._Instance.PopupInputField(trackInfo.Title, "Name your Preset", "Confirm Preset Name", "Use Track Title", false,
            x => { 
                string path = SaveManager._Instance.SavePreset(x, preset);
                UIManager._Instance.AddNewPopupMessage("Preset saved to " + path);
            }, 
            () => SaveManager._Instance.SavePreset(trackInfo.Title, preset));
    }

    public void LoadPreset(string filePath, Action<string, VisualizerPreset> onSuccess, Action<string> onFailure)
    {
        try
        {
            VisualizerPreset preset = SaveManager._Instance.LoadPreset(filePath);

            // Set colors
            ClearColorsList();
            trackInfo.Colors = preset.Colors;
            trackInfo.Gradients = preset.Gradients;
            PopulateColorsList();

            // Set fonts
            loadedFontData.Clear();
            loadedFontIndices.Clear();
            loadedTMPFontAssets.Clear();
            ClearFontsList();
            for (int i = 0; i < preset.Fonts.Count; i++)
            {
                FontFileData fontData = preset.Fonts[i];
                loadedTMPFontAssets.Add(fontData.FontName, LoadTMPFontFromFontAsset(LoadFontFromByteArray(fontData.FileContents)));
                loadedFontIndices.Add(fontData.FontName, i);
                loadedFontData.Add(fontData.FontName, fontData);

                // Create UI
                FontListElement spawned = Instantiate(fontListElement, fontsList);
                spawned.Set(i);
            }

            // Set visualizer elements
            visualizerElementsInfo = preset.VisualizerElements;

            BroadcastTrackInfo();
            BroadcastVisualizerElementsInfo();

            onSuccess(filePath, preset);
        } catch (Exception e)
        {
            Debug.LogError(e.ToString());
            onFailure(filePath);
        }
    }
}
