using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Rendering.Universal;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.Networking;

public enum AudioChannel
{
    STEREO,
    LEFT,
    RIGHT
}

[RequireComponent(typeof(AudioSource))]
public class VisualizerManager : MonoBehaviour
{
    public static VisualizerManager _Instance { get; private set; }

    [Header("Scenarios")]
    [SerializeField] private List<SerializableKeyValuePair<string, GameObject>> scenarios = new();
    [SerializeField] private GameObject defaultScenary;
    [SerializeField] private GameObject scenarioSelection;

    [Header("Track Info")]
    [SerializeField] private TrackInfo preset;
    private string trackName;
    private Sprite trackArt;
    private List<Color> trackColors = new();
    private List<Gradient> trackGradients = new();
    private List<TMP_FontAsset> trackFonts = new();

    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private float audioSampleSmoothing = 100;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    [Header("Other Settings")]
    [SerializeField] private bool usePreset;
    [SerializeField] private bool askForTrack = true;
    [SerializeField] private bool askForCoverArt = true;
    [SerializeField] private bool askForColors = true;
    [SerializeField] private bool askForFonts = true;
    [SerializeField] private bool askForVisualizerElements = true;
    [SerializeField] private bool startImmedietelyUponLoadingTrack;
    [SerializeField] private float startSongAtSeconds;
    [SerializeField] private TransitionData initialTransition;
    private bool hasSongStarted = false;
    private float lastAudioSourceTime;

    [Header("Recording Settings")]
    [SerializeField] private float afterTrackRecordingBufferTime = 10f;

    [Header("Blur Settings")]
    [SerializeField] private KawaseBlurSettings blurSettings;

    [Header("References")]
    [SerializeField] private CanvasGroup visualizerCanvasGroup;
    [SerializeField] private GameObject visualizerElementsUI;
    [SerializeField] private UniversalRendererData urpData;
    [SerializeField] private TMP_FontAsset defaultFont;

    [Header("Visualizer Elements")]
    [SerializeField] private List<SerializableKeyValuePair<SetupElement, SetElementColorToMatchTrack>> visualizerColorElementDict = new();
    [SerializeField] private List<SerializableKeyValuePair<SetupElement, SetFontToVisualizerFont>> visualizerFontElementDict = new();

    [Header("Colors")]
    [SerializeField] private Transform colorsList;
    [SerializeField] private GameObject colorsUI;
    [SerializeField] private Transform fontsList;
    [SerializeField] private GameObject fontsUI;

    [Header("Prefabs")]
    [SerializeField] private ColorListElement colorListElement;
    [SerializeField] private FontListElement fontListElement;
    private KawaseBlur kawaseBlurPass;

    private Color blankColor = new Color(0, 0, 0, 0);
    private Gradient blankGradient = new Gradient();

    // Events
    public Action OnSongEnd;
    public Action OnSongStart;

    private AudioSource audioSource;

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

    [ContextMenu("RebroadcastTrackInfo")]
    private void BroadcastTrackInfo()
    {
        // Find all listeners
        if (trackInfoListeners.Count == 0)
        {
            trackInfoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTrackInfo>().ToList();
        }

        // Create Track Info struct
        TrackInfo trackInfo = new TrackInfo(trackName, trackArt, StringHelper.GetDurationText(audioSource.clip.length), 
            audioSource.clip, trackColors, trackGradients, trackFonts);

        // Send data out
        trackInfoListeners.ForEach(item => item.RecieveTrackInfo(trackInfo));
    }

    private List<IRecieveTrackInfo> trackInfoListeners = new();

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        // Get audio source component
        audioSource = GetComponent<AudioSource>();

        trackFonts.Add(defaultFont);

        LoadPreset();
    }

    private void Start()
    {
        PopulateColorsList();
        PopulateFontsList();

        // Only 1 scenario, we'd just go to track selection
        if (scenarios.Count == 0)
        {
            StartCoroutine(RunSetup());
        }
    }

    // Update is called once per frame
    private void Update()
    {
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

        // Create audio profile
        CreateAudioProfile();

        // Inform listeners of track
        BroadcastTrackInfo();

        // Fetch Kawase Blur Feature
        kawaseBlurPass = (KawaseBlur)urpData.rendererFeatures[0];

        // Change Settings
        SetKawaseBlurFeatureSettings();

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
        return GetColor(type, index, Vector2.zero);
    }

    public Color GetColor(VisualizerColorType type, int index, Vector2 positionalData)
    {
        switch (type)
        {
            case VisualizerColorType.COLOR:
                return GetTrackColor(index);
            case VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(positionalData.x / positionalData.y);
            case VisualizerColorType.TIME_BASED_GRADIENT:
                return GetTrackGradient(index).Evaluate(PlaythroughPercent);
            default:
                throw new System.Exception(); // TODO: Custom Exception
        }
    }

    public Color GetTrackColor(int index) 
    {
        if (index > trackColors.Count - 1) return blankColor;
        return trackColors[index]; 
    
    }
    public Gradient GetTrackGradient(int index) 
    {
        if (trackGradients.Count - 1 == 0) return blankGradient;
        return trackGradients[index]; 
    }

    public TMP_FontAsset GetFont(int index)
    {
        if (index > trackFonts.Count - 1) return defaultFont;
        return trackFonts[index];
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
        audioSource.time = 0;
        PausePlayback();
    }

    public void Seek(float amount)
    {
        audioSource.time += amount;
    }

    private void SetKawaseBlurFeatureSettings()
    {
        kawaseBlurPass.SetActive(blurSettings.Enabled);
        kawaseBlurPass.settings.blurPasses = blurSettings.BlurPasses;
        kawaseBlurPass.settings.downsample = blurSettings.Downsample;
        kawaseBlurPass.settings.copyToFramebuffer = blurSettings.CopyToFrameBuffer;
    }

    public void SelectScenario(string name)
    {
        defaultScenary.SetActive(false);
        scenarioSelection.SetActive(false); ;

        foreach (SerializableKeyValuePair<string, GameObject> kvp in scenarios)
        {
            kvp.Value.SetActive(kvp.Key == name);
        }

        if (usePreset)
        {
            SetupComplete();
        } else
        {
            StartCoroutine(RunSetup());
        }
    }

    private void LoadPreset()
    {
        audioSource.clip = preset.AudioClip;
        trackName = preset.Title;
        trackArt = preset.CoverArt;
        trackColors = preset.Colors;
        trackGradients = preset.Gradients;
    }

    private IEnumerator RunSetup()
    {
        if (askForTrack)
            yield return StartCoroutine(RunTrackSelection());

        if (askForCoverArt)
            yield return StartCoroutine(RunCoverArtSelection());

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
                    trackName = StringHelper.GetFileName(filePath);

                },
                x => Debug.Log("Failed to Load Audio Clip from path = " + x)));
        }, "Select Track", "Load"));

        BroadcastTrackInfo();
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
                    trackArt = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                },
            x => Debug.Log("Failed to Load Image from path = " + x));
        }, "Select Cover Art", "Load"));

        BroadcastTrackInfo();
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

        yield return new WaitUntil(() => !visualizerElementsUI.activeSelf);

        SetVisualizerElements();

        BroadcastTrackInfo();
    }

    public IEnumerator SelectOneFont(Action<string, TMP_FontAsset> onSuccess)
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Font", ".ttf", ".otf"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            TMP_FontAsset font = LoadFontFromFile(x);
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
        throw new Exception(); // TODO: Custom Exception
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

    private TMP_FontAsset LoadFontFromFile(string filePath)
    {
        Debug.Log("Loading Font from File: " + filePath);
        Font font = new Font(filePath);
        return TMP_FontAsset.CreateFontAsset(font);
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

    public void AddColorElement()
    {
        trackColors.Add(Color.white);
        ColorListElement spawned = Instantiate(colorListElement, colorsList);
        spawned.Set(trackColors.Count - 1, Color.white);
    }

    private void PopulateColorsList()
    {
        for (int i = 0; i < trackColors.Count; ++i)
        {
            ColorListElement spawned = Instantiate(colorListElement, colorsList);
            spawned.Set(i, trackColors[i]);
        }
    }

    public void UpdateTrackColor(int index, Color c)
    {
        if (index > trackColors.Count - 1) return;
        trackColors[index] = c;
    }

    public void AddFontElement()
    {
        StartCoroutine(BrowseForMultipleFiles(x =>
        {
            TMP_FontAsset font = LoadFontFromFile(x);
            FontListElement spawned = Instantiate(fontListElement, fontsList);
            spawned.Set(trackFonts.Count, font, x);
            trackFonts.Add(font);
        }, "Choose Fonts", "Load"));
    }

    private void PopulateFontsList()
    {
        for (int i = 0; i < trackFonts.Count; ++i)
        {
            FontListElement spawned = Instantiate(fontListElement, fontsList);
            spawned.Set(i, trackFonts[i]);
        }
    }

    public void UpdateFont(int index, TMP_FontAsset font)
    {
        if (index > trackFonts.Count - 1) return;
        trackFonts[index] = font;
    }

    private void SetVisualizerElements()
    {
        SetVisualizerColorElements();
        SetVisualizerFontElements();
    }

    private void SetVisualizerColorElements()
    {
        foreach (SerializableKeyValuePair<SetupElement, SetElementColorToMatchTrack> kvp in visualizerColorElementDict)
        {
            kvp.Value.Active = kvp.Key.Active;
            kvp.Value.ColorIndex = kvp.Key.ColorIndex;
        }
    }

    private void SetVisualizerFontElements()
    {
        foreach (SerializableKeyValuePair<SetupElement, SetFontToVisualizerFont> kvp in visualizerFontElementDict)
        {
            kvp.Value.FontIndex = kvp.Key.FontIndex;
        }
    }

    public void ChangeVolume(float amount)
    {
        audioSource.volume += amount;
    }
}
