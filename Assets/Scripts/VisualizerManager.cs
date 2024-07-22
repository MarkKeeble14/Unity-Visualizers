using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;
using SoundCloudExplode;
using SoundCloudExplode.Tracks;
using System.Threading.Tasks;
using AForge.Math;

[System.Serializable]
public struct AudioClipData
{
    [SerializeField] public int Channels;
    [SerializeField] public int Frequency;
    [SerializeField] public float[] Samples;
    [SerializeField] public bool Set;

    public AudioClipData(int channels, int frequency, float[] samples)
    {
        Channels = channels;
        Frequency = frequency;
        Samples = samples;
        Set = true;
    }
}

[System.Serializable]
public struct SpriteData
{
    [SerializeField] public byte[] PNGEncodedData;
    [SerializeField] public int Width;
    [SerializeField] public int Height;
    [SerializeField] public TextureFormat TextureFormat;
    [SerializeField] public bool Set;

    public SpriteData(byte[] rawData, int width, int height, TextureFormat textureFormat)
    {
        PNGEncodedData = rawData;
        Width = width;
        Height = height;
        TextureFormat = textureFormat;
        Set = true;
    }
}

[System.Serializable]
public struct VisualizerElementsSettings
{
    [SerializeField, HideInInspector] public VisualizerColorType ColorType;
    [SerializeField, HideInInspector] public int ColorIndex;
    [SerializeField, HideInInspector] public int FontIndex;
    [SerializeField, HideInInspector] public bool Enabled;

    public VisualizerElementsSettings(VisualizerColorType colorType, int colorIndex, int fontIndex, bool enabled)
    {
        ColorType = colorType;
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
    [SerializeField] private Color defaultColor;
    [SerializeField] private Gradient defaultGradient;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TrackInfo trackInfo;
    public string TrackTitle => trackInfo.Title;

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
    [SerializeField] private bool setDefaultColorsFromTexture = true;
    [SerializeField] private bool startImmedietelyUponLoadingTrack;
    [SerializeField] private bool applyPostProcessing;
    [SerializeField, Range(0, 1)] private float volumeWeight;

    [SerializeField] private List<SerializableKeyValuePair<string, float>> startingFloatSettings = new();
    [SerializeField] private List<SerializableKeyValuePair<string, int>> startingIntSettings = new();
    [SerializeField] private List<SerializableKeyValuePair<string, bool>> startingBoolSettings = new();

    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    private int numSamples = 512;

    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    [Header("Smoothing")]
    [SerializeField] private bool enableSmoothing;
    [SerializeField] private float smoothingEquationStrength = .5f;
    [SerializeField] private float smoothingEquationShift = 1;
    [SerializeField] private float smoothingEquationScale = 128;
    private float cachedSmoothingValue;

    [Header("Normalizing")]
    [SerializeField] private bool normalizeSamples;
    [SerializeField] private float minNormalizedSampleValue = 0;
    [SerializeField] private float maxNormalizedSampleValue = 1;
    private float cachedSmallestValueLeft = Mathf.Infinity;
    private float cachedLargestValueLeft = 0;
    private float cachedSmallestValueRight = Mathf.Infinity;
    private float cachedLargestValueRight = 0;

    [Header("Tapping")]
    [SerializeField] private int tapBufferCount = 5;

    [Header("BPM Calculations")]
    [SerializeField] private float varianceSensitivity = 1.3f;
    private float energyBufferSize;
    private float currentEnergy;
    private List<float> energyBuffer = new();
    private float localEnergy;
    private float averageLocalEnergy;

    [SerializeField] private int measureBPMOverInterval = 15;
    private List<int> beatCountsPerSecondBuffer = new();
    private int beatsLastSecond;
    private float estimatedBPM;
    private bool estimatingBPM;
    private bool tappingBPM;
    private bool didTap;

    [Header("Controls")]
    [SerializeField] private ControlScheme activeControlScheme = ControlScheme.VISUALIZER;
    public ControlScheme ActiveControlScheme => activeControlScheme;

    [Header("Beat Detection Settings")]
    [SerializeField] private float ampSpikeDetectionSensitivity = 0.9f;

    [Header("Transition Settings")]
    [SerializeField] private TransitionData initialTransition;

    [Header("References")]
    [SerializeField] private CanvasGroup visualizerCanvasGroup;
    [SerializeField] private GameObject tempoMenuUI;
    [SerializeField] private TMP_InputField tempoTapperInputField;
    [SerializeField] private Button tempoTapperButton;
    [SerializeField] private GameObject baseVisualizerElementsUI;
    [SerializeField] private GameObject visualizerSpecificElementsUI;
    [SerializeField] private GameObject colorsUI;
    [SerializeField] private GameObject fontsUI;

    [SerializeField] private Transform colorsList;
    [SerializeField] private Transform fontsList;
    [SerializeField] private Button loadingTrackButton;
    [SerializeField] private Button loadingImageButton;
    [SerializeField] private EscapeMenuFunctions escapeMenu;
    [SerializeField] private Volume volume;
    [SerializeField] private UniversalAdditionalCameraData activeCameraAdditionalCameraData;
    private AudioSource audioSource;

    [Header("Prefabs")]
    [SerializeField] private ColorListElement colorListElement;
    [SerializeField] private GradientListElement gradientListElement;
    [SerializeField] private FontListElement fontListElement;

    private List<IRecieveTrackInfo> trackInfoListeners = new();
    private List<IRecieveTempo> tempoListeners = new();
    private List<IRecieveControlScheme> controlSchemeListeners = new();
    private List<IRecieveVisualizerElementsInfo> visualizerElementsInfoListeners = new();
    private List<IRecieveVisualizerSpecificElementsInfo> visualizerSpecificElementsInfoListeners = new();
    private List<IRecieveVisualizerFloatValues> visualizerFloatValueListeners = new();
    private List<IRecieveVisualizerIntValues> visualizerIntValueListeners = new();
    private List<IRecieveVisualizerBoolValues> visualizerBoolValueListeners = new();
    private Dictionary<VisualizerElementLabel, VisualizerElementsSettings> baseVisualizerElementsInfo = new();
    private Dictionary<string, VisualizerElementsSettings> visualizerSpecificElementsInfo = new();

    private Dictionary<string, float> visualizerFloatValues = new();
    private Dictionary<string, int> visualizerIntValues = new();
    private Dictionary<string, bool> visualizerBoolValues = new();

    private bool hasSongStarted;
    private float lastAudioSourceTime;
    private float prePlaybackPositionTracker;

    // Events
    public Action OnSongEnd;
    public Action OnSongStart;
    public Action<float> OnSnareHit;
    public Action<float> OnEnergySpike;
    public Action<float> OnNewAmplitudePeak;

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

    public float PlaythroughPercent
    {
        get
        {
            if (hasSongStarted)
            {
                if (audioSource.clip == null) { return 0; }
                return audioSource.time / audioSource.clip.length;
            } else
            {
                return prePlaybackPositionTracker;
            }
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

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        // Get audio source component
        audioSource = GetComponent<AudioSource>();

        PopulateColorsList();
        PopulateFontsList();

        // populate dictionaries
        foreach (VisualizerElementLabel item in Enum.GetValues(typeof(VisualizerElementLabel)))
        {
            baseVisualizerElementsInfo.Add(item, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
        }

        foreach (SerializableKeyValuePair<string, float> kvp in startingFloatSettings)
        {
            visualizerFloatValues.Add(kvp.Key, kvp.Value);
        }
        foreach (SerializableKeyValuePair<string, int> kvp in startingIntSettings)
        {
            visualizerIntValues.Add(kvp.Key, kvp.Value);
        }
        foreach (SerializableKeyValuePair<string, bool> kvp in startingBoolSettings)
        {
            visualizerBoolValues.Add(kvp.Key, kvp.Value);
        }
    }

    private void Start()
    {
        BroadcastControlScheme();
    }

    // Update is called once per frame
    private void Update()
    {
        // Enable/disable post processing
        if (activeCameraAdditionalCameraData != null)
            activeCameraAdditionalCameraData.renderPostProcessing = applyPostProcessing;

        // Change weight
        volume.weight = volumeWeight;

        if (audioSource.isPlaying)
        {
            // Spectrum Data
            if (enableSmoothing)
            {
                GetSmoothedSpectrumData();
            } else
            {
                OnlyGetSpectrumData();
            }

            if (normalizeSamples)
            {
                NormalizeSamples();
            }

            // Make Frequency Bands
            MakeFrequencyBands();

            // Band Buffer
            CalcBandBuffer();

            // Create Audio Bands
            CreateAudioBands();

            // Calculate Amplitude
            GetAmplitude();

            // Check Beat
            CheckBeat();
        }

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
        // Create audio profile
        CreateAudioProfile();

        // Inform listeners of track
        BroadcastTrackInfo();

        // Play initial transition if there is one
        if (initialTransition.transition != null)
        {
            initialTransition.transition.InitiateTransition(initialTransition.direction);
        }
    }

    [ContextMenu("Reset Sample Normalization Values")]
    private void ResetSampleNormalizationValues()
    {
        cachedSmallestValueLeft = Mathf.Infinity;
        cachedLargestValueLeft = 0;
        cachedSmallestValueRight = Mathf.Infinity;
        cachedLargestValueRight = 0;
    }

    private void NormalizeSamples()
    {
        // reset values for normalizing
        ResetSampleNormalizationValues();

        // initial pass
        for (int i = 0; i < numSamples; ++i)
        {
            if (leftAudioSamples[i] < cachedSmallestValueLeft) cachedSmallestValueLeft = leftAudioSamples[i];
            if (leftAudioSamples[i] > cachedLargestValueLeft) cachedLargestValueLeft = leftAudioSamples[i];
            if (rightAudioSamples[i] < cachedSmallestValueRight) cachedSmallestValueRight = rightAudioSamples[i];
            if (rightAudioSamples[i] > cachedLargestValueRight) cachedLargestValueRight = rightAudioSamples[i];
        }

        // secondary pass
        for (int i = 0; i < numSamples; ++i)
        {
            leftAudioSamples[i] = MathHelper.Normalize(leftAudioSamples[i], cachedSmallestValueLeft, cachedLargestValueLeft, minNormalizedSampleValue, maxNormalizedSampleValue);
            rightAudioSamples[i] = MathHelper.Normalize(rightAudioSamples[i], cachedSmallestValueRight, cachedLargestValueRight, minNormalizedSampleValue, maxNormalizedSampleValue);
        }
    }

    private void GetSmoothedSpectrumData()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);

        // initial pass
        for (int i = 0; i < numSamples; ++i)
        {
            // smooth
            cachedSmoothingValue = GetSmoothingValue(i, numSamples);
            leftAudioSamples[i] = leftAudioSamples[i] * cachedSmoothingValue;
            rightAudioSamples[i] = rightAudioSamples[i] * cachedSmoothingValue;
        }
    }

    private void OnlyGetSpectrumData()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);
    }

    private float GetSmoothingValue(int i, int total)
    {
        return Mathf.Pow((((float)i / total) * smoothingEquationScale) + smoothingEquationShift, 2) * smoothingEquationStrength;
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

        float ampSpikeThreshold = highestAmplitude * ampSpikeDetectionSensitivity;
        if (amplitude > ampSpikeThreshold)
        {
            OnNewAmplitudePeak?.Invoke(amplitude - ampSpikeThreshold);
        }
    }

    private void CreateAudioProfile()
    {
        for (int i = 0; i < highestValuePerFrequencyBand.Length; i++)
            highestValuePerFrequencyBand[i] = beginningHighestFrequencyBandValue;
    }

    private void CheckBeat()
    {
        energyBufferSize = measureBPMOverInterval * 60;
        currentEnergy = 0;
        for (int i = 0; i < 512; ++i)
        {
            switch (channel)
            {
                case AudioChannel.STEREO:
                    currentEnergy += leftAudioSamples[i] + rightAudioSamples[i];
                    break;
                case AudioChannel.LEFT:
                    currentEnergy += leftAudioSamples[i];
                    break;
                case AudioChannel.RIGHT:
                    currentEnergy += rightAudioSamples[i];
                    break;
            }
        }

        // calculate local energy
        localEnergy = 0;
        foreach (float v in energyBuffer) { localEnergy += v; }

        // calculate average local energy
        averageLocalEnergy = localEnergy / energyBufferSize;

        // add current energy to energy buffer
        energyBuffer.Add(currentEnergy);

        // if there are more samples in the energy buffer than are allowed, remove the oldest value
        if (energyBuffer.Count > energyBufferSize) { energyBuffer.RemoveAt(0); }

        // check for energy spike
        if (currentEnergy > averageLocalEnergy * varianceSensitivity)
        {
            OnEnergySpike?.Invoke(currentEnergy);
        }
    }

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }
    public float GetEstimatedBPM() { return estimatedBPM; }

    public void BeginPlayback()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("Attempted to start the Visualizer with no track loaded");
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, 
                "Attempt to start Visualizer with no track loaded ignored - Please load a track");
            return;
        }

        // Play the Track
        audioSource.Play();

        if (!hasSongStarted)
        {
            audioSource.time = prePlaybackPositionTracker * audioSource.clip.length;
        }
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
        if (index > trackInfo.Colors.Count - 1) return defaultColor;
        return trackInfo.Colors[index];

    }

    public Gradient GetTrackGradient(int index)
    {
        if (index > trackInfo.Gradients.Count - 1) return defaultGradient;
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

    public Color GetDefaultColor()
    {
        return defaultColor;
    }

    public Gradient GetDefaultGradient()
    {
        return defaultGradient;
    }

    public TMP_FontAsset GetDefaultFont()
    {
        return defaultFont;
    }

    public string GetFontName(int index)
    {
        return loadedFontData[GetFontKeyAtIndex(index)].FontName;
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

    public void SetPlaythroughPosition(float v)
    {
        Debug.Log("SetPlaythroughPosition: " + v);
        if (!hasSongStarted)
        {
            prePlaybackPositionTracker = v;
            return;
        }

        if (audioSource.clip == null)
        {
            return;
        }

        v *= audioSource.clip.length;
        if (v < audioSource.clip.length)
        {
            audioSource.time = v;
        } else
        {
            audioSource.time = audioSource.clip.length - 1;
        }
    }

    public void Seek(float amount)
    {
        audioSource.time += amount;
    }

    public void ChangeVolume(float amount)
    {
        audioSource.volume += amount;
    }

    public IEnumerator RunFontsSelection()
    {
        fontsUI.SetActive(true);

        yield return new WaitUntil(() => !fontsUI.activeInHierarchy);

        BroadcastTrackInfo();
    }

    public void ChooseAudio()
    {
        StartCoroutine(RunTrackSelection());
    }

    public void ChooseCoverArt()
    {
        StartCoroutine(RunCoverArtSelection());
    }

    public void EditColors()
    {
        StartCoroutine(RunColorsSelection());
    }

    public void EditFonts()
    {
        StartCoroutine(RunFontsSelection());
    }

    public void LoadPresetFromFile()
    {
        StartCoroutine(RunLoadPresetSelection());
    }

    #region Track Selection

    public IEnumerator RunTrackSelection()
    {
        loadingTrackButton.interactable = false;

        yield return UIManager._Instance.PopupActionSelection("Load Track from URL or File?", "Cancel", 
        () =>
        {
            loadingTrackButton.interactable = true;
        }, new List<ActionSelection>()
        {
            new ActionSelection("URL", null, PopoutEnterTrackURL()),
            new ActionSelection("File", null, BrowseForTrack())
        });
    }

    private IEnumerator PopoutEnterTrackURL()
    {
        yield return UIManager._Instance.PopupInputField("URL", "Enter YouTube URL", "Confirm", "Cancel", false,
                    AttemptToDownloadTrackFromURL, OnFailDownloadTrackFromURL());
    }

    private IEnumerator OnFailDownloadTrackFromURL()
    {
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to download Track from URL");

        loadingTrackButton.interactable = true;

        yield return null;
    }

    private string GetSupportedOrigins()
    {
        return "[ YouTube ] ";
    }

    private IEnumerator AttemptToDownloadTrackFromURL(string url)
    {
        int loadingKey = UIManager._Instance.AddLoading("Downloading Track from URL");

        ImportableAudioSource origin;
        try
        {
            origin = GetAudioFileOrigin(url);
        } catch (Exception e)
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to download track from URL");

            UIManager._Instance.RemoveLoading(loadingKey);

            loadingTrackButton.interactable = true;

            yield break;
        }
        switch (origin)
        {
            case ImportableAudioSource.SOUNDCLOUD:
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Unsupported Origin, Supported Origins are: " + GetSupportedOrigins());

                UIManager._Instance.RemoveLoading(loadingKey);

                loadingTrackButton.interactable = true;

                Debug.Log("Unsupported Origin: " + origin);
                break;
                /*
                yield return StartCoroutine(DownloadTrackFromSoundCloud(url, false,
                    (url, name, clip) =>
                    {
                        SetTrack(url, clip, name);
                    }, null));
                break;
                */
            case ImportableAudioSource.YOUTUBE:
                yield return StartCoroutine(DownloadTrackFromYouTube(url,
                    (url, clip, name, duration) =>
                    {
                        SetTrack(url, clip, name, duration);

                        UIManager._Instance.RemoveLoading(loadingKey);

                        loadingTrackButton.interactable = true;
                    }, null));
                break;
            default:
                Debug.Log("Unsupported Origin: " + origin);
                break;
        }
    }

    private ImportableAudioSource GetAudioFileOrigin(string url)
    {
        string[] f = url.Split("https://");
        if (f.Length < 2)
        {
            // error
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "URL could not be parsed - Please try again with a different URL");
        }
        url = f[1];

        string[] f2 = url.Split('/');
        if (f.Length < 2)
        {
            // error
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "URL could not be parsed - Please try again with a different URL");
        }
        url = f2[0].ToLower();

        if (url.Contains("soundcloud"))
            return ImportableAudioSource.SOUNDCLOUD;
        else if (url.Contains("youtube"))
            return ImportableAudioSource.YOUTUBE;
        else
        {
            throw new UncaughtSwitchTypeException(typeof(ImportableAudioSource)); // TODO: Custom Exceptions
        }
    }

    private ValueTask<string[]> YouTubeToMP3(string mediaUrl, string directoryPath)
    {
        YouTube youtube = YouTube.Default;
        YouTubeVideo vid = youtube.GetVideo(mediaUrl);

        // determine mp4 path
        string inputFilePath = Path.Combine(directoryPath, "tmp");
        inputFilePath = inputFilePath.Replace("/", @"\");

        // determine mp3 path & add extensions
        string outputFilePath = inputFilePath + ".mp3";
        inputFilePath += ".mp4";

        // write to video file
        File.WriteAllBytes(inputFilePath, vid.GetBytes());

        MediaFile inputFile = new MediaFile { Filename = inputFilePath };
        MediaFile outputFile = new MediaFile { Filename = outputFilePath };
        
        // convert mp4 to mp3
        using (var engine = new Engine())
        {
            engine.GetMetadata(inputFile);

            engine.Convert(inputFile, outputFile);

            engine.GetMetadata(outputFile);
        }

        return new ValueTask<string[]>(new string[] { inputFilePath, outputFilePath, vid.Title, vid.Info.LengthSeconds.ToString() });
    }

    private async void DownloadTrackFromYouTubeAsync(string mediaUrl, Action<string, AudioClip, string, string> onSuccess, Action<string> onFailure)
    {
        // create folder if neccessary
        string directoryPath = Path.Combine(Application.dataPath, "../temp");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        int loadingKey = UIManager._Instance.AddLoading("Fetching audio from URL...");

        var task = Task.Run(async () => await YouTubeToMP3(mediaUrl, directoryPath));
        await task;

        UIManager._Instance.RemoveLoading(loadingKey);

        string mp4FilePath = task.Result[0];
        string mp3FilePath = task.Result[1];
        string trackName = task.Result[2];
        string trackDuration = task.Result[3];

        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Successfully fetched Audio from YouTube Video: " + trackName);

        // delete input file
        File.Delete(mp4FilePath);

        // Delete output files
        onSuccess += (path, name, duration, clip) => File.Delete(mp3FilePath);
        onFailure += path => File.Delete(mp3FilePath);

        // load data from output file
        StartCoroutine(LoadAudioClipFromFile(mp3FilePath, 
            (path, clip) =>
            {
                onSuccess?.Invoke(path, clip, trackName, trackDuration);
            }, onFailure));
    }

    private IEnumerator DownloadTrackFromYouTube(string mediaUrl, Action<string, AudioClip, string, string> onSuccess, Action<string> onFailure)
    {
        DownloadTrackFromYouTubeAsync(mediaUrl, onSuccess, onFailure);
        yield return null;
    }

    private IEnumerator DownloadTrackFromYouTubeWait(string mediaUrl, Action<string, AudioClip, string, string> onSuccess, Action<string> onFailure)
    {
        bool completed = false;
        onSuccess += (filePath, name, duration, clip) => completed = true;
        onFailure += filePath => completed = true;
        DownloadTrackFromYouTubeAsync(mediaUrl, onSuccess, onFailure);
        yield return new WaitUntil(() => completed);
    }

    private async void DownloadTrackFromSoundCloudAsync(string mediaUrl, bool useCover, Action<string, string, AudioClip> onSuccess, Action<string> onFailure)
    {
        SoundCloudClient soundcloud = new SoundCloudClient();

        // add UI
        string tempTitle = mediaUrl.Split("https://soundcloud.com/")[1];
        int loadingKey = UIManager._Instance.AddLoading("Attempting to fetch: " + tempTitle);

        // setup fetch
        var getTask = Task.Run(async () => await soundcloud.Tracks.GetAsync(mediaUrl));
        await getTask;

        // remove UI
        UIManager._Instance.RemoveLoading(loadingKey);

        // add UI
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Successfully fetched: " + tempTitle);

        // get track from recieved data
        Track track = getTask.Result;

        // create folder if neccessary
        string directoryPath = Path.Combine(Application.dataPath, "../temp");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // determine path
        string outputFilePath = Path.Combine(directoryPath, track.Title) + ".mp3";
        outputFilePath = outputFilePath.Replace("/", @"\");

        // add UI
        loadingKey = UIManager._Instance.AddLoading("Attempting to download: " + track.Title);

        // download the data
        var downloadTask = Task.Run(async () => await soundcloud.DownloadAsync(track, outputFilePath));

        await downloadTask;

        // add UI
        UIManager._Instance.RemoveLoading(loadingKey);

        if (useCover)
        {
            // load cover art on success as well
            onSuccess += (path, name, clip) => StartCoroutine(AttemptToDownloadImageFromURL(track.ArtworkUrl.ToString()));
        }

        StartCoroutine(LoadAudioClipFromFile(outputFilePath, (path, clip) =>
        {
            onSuccess?.Invoke(path, track.Title, clip);
        }, onFailure));

        // delete file
        File.Delete(outputFilePath);
    }

    private IEnumerator DownloadTrackFromSoundCloud(string mediaUrl, bool useCover, Action<string, string, AudioClip> onSuccess, Action<string> onFailure)
    {
        DownloadTrackFromSoundCloudAsync(mediaUrl, useCover, onSuccess, onFailure);
        yield return null;
    }

    private IEnumerator DownloadTrackFromSoundCloudWait(string mediaUrl, bool useCover, Action<string, string, AudioClip> onSuccess, Action<string> onFailure)
    {
        bool completed = false;
        onSuccess += (filePath, name, clip) => completed = true;
        onFailure += filePath => completed = true;
        DownloadTrackFromSoundCloudAsync(mediaUrl, useCover, onSuccess, onFailure);
        yield return new WaitUntil(() => completed);
    }

    private IEnumerator BrowseForTrack()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Audio from File: " + x);

            StartCoroutine(LoadAudioClipFromFile(x,
                (filePath, clip) =>
                {
                    SetTrack(filePath, clip);
                },
                x =>
                {
                    UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load track from selected file " +
                        "- Ensure the file selected is an appropriate file type");
                    Debug.Log("Failed to Load Audio Clip from path = " + x);
                }));
        }, "Select Track", "Load"));

        loadingTrackButton.interactable = true;
    }

    // duration will take the format of mm:ss
    private void SetTrack(string filePath, AudioClip clip, string trackName = "", string durationSeconds = "")
    {
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Track Set");

        audioSource.clip = clip;

        if (string.IsNullOrEmpty(trackName))
        {
            trackInfo.Title = StringHelper.GetFileName(filePath);
        } else
        { 
            trackInfo.Title = trackName;
        }

        if (string.IsNullOrEmpty(durationSeconds))
        {
            trackInfo.Duration = StringHelper.GetDurationText(audioSource.clip.length);
        }
        else
        {
            float v;
            if (float.TryParse(durationSeconds, out v))
            {
                trackInfo.Duration = StringHelper.GetDurationText(v);
            } else
            {
                Debug.Log("Unable to parse duration - ensure durationSeconds represents a numerical value");
            }
        }

        Debug.Log("Setting track to " + trackInfo.Title);

        if (startImmedietelyUponLoadingTrack)
        {
            BeginPlayback();
        }

        BroadcastTrackInfo();
    }

    public void UpdateTrackTitle(string s)
    {
        trackInfo.Title = s;

        BroadcastTrackInfo();
    }

    #endregion

    #region Cover Art Selection
    public IEnumerator RunCoverArtSelection()
    {
        loadingImageButton.interactable = false;

        yield return UIManager._Instance.PopupActionSelection("Load Art from URL or File?", "Cancel", 
        () =>
        {
            loadingImageButton.interactable = true;
        }, new List<ActionSelection>()
        {
            new ActionSelection("URL", null, PopoutEnterImageURL()),
            new ActionSelection("File", null, BrowseForImage())
        });
    }

    private IEnumerator PopoutEnterImageURL()
    {
        yield return UIManager._Instance.PopupInputField("URL", "Enter Image URL", "Confirm", "Cancel", false,
                    AttemptToDownloadImageFromURL, OnFailDownloadImageFromURL());
    }

    private IEnumerator AttemptToDownloadImageFromURL(string url)
    {
        int loadingKey = UIManager._Instance.AddLoading("Downloading Image from URL");

        yield return StartCoroutine(DownloadImage(url,
                (url, tex) =>
                {
                    UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Successfully downloaded image from: " + url);
                    SetCoverArt(tex);
                },
                url =>
                {
                    UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to download image from: " + url);
                }));

        UIManager._Instance.RemoveLoading(loadingKey);

        loadingImageButton.interactable = true;
    }

    private IEnumerator OnFailDownloadImageFromURL()
    {
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to download Image from URL");

        loadingImageButton.interactable = true;

        yield return null;
    }

    private IEnumerator BrowseForImage()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpeg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Image from File: " + x);
            LoadImageFromFile(x,
                (filePath, texture) =>
                {
                    Debug.Log("Successfully Loaded Image from path = " + x);
                    SetCoverArt(texture);
                },
            x =>
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load image from selected file " +
                    "- Ensure the file selected is an appropriate file type");
                Debug.Log("Failed to Load Image from path = " + x);
            });
        }, "Select Cover Art", "Load"));

        loadingImageButton.interactable = true;
    }

    private void SetCoverArt(Texture2D texture)
    {
        trackInfo.CoverArt = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        Debug.Log("Setting track cover");
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Cover Art Set");

        if (setDefaultColorsFromTexture)
        {
            defaultColor = AverageColorFromTexture(texture);
            defaultGradient = new Gradient();
            defaultGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(defaultColor, 0), new GradientColorKey(Color.white, 1) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
        }

        BroadcastTrackInfo();
    }

    private Color32 AverageColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
    }

    private IEnumerator DownloadImage(string mediaUrl, Action<string, Texture2D> onSuccess, Action<string> onFailure)
    {
        UIManager._Instance.AddNewMessage(UIManager.MessageClass.INFO, "Attempting to Download Image from: " + mediaUrl);

        Texture2D tex;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                onFailure(mediaUrl);
            }
            else
            {
                tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                onSuccess(mediaUrl, tex);
            }
        }
    }

    #endregion

    public IEnumerator RunColorsSelection()
    {
        // Enable UI
        colorsUI.SetActive(true);

        yield return new WaitUntil(() => !colorsUI.activeSelf);

        BroadcastTrackInfo();
    }

    public IEnumerator RunEditBaseVisualizerElements()
    {
        BroadcastSetupValues();

        baseVisualizerElementsUI.SetActive(true);

        yield return new WaitUntil(() => !baseVisualizerElementsUI.activeSelf);

        BroadcastSetupValues();
        BroadcastTrackInfo();
    }

    public IEnumerator RunEditVisualizerSpecificElements()
    {
        BroadcastSetupValues();

        visualizerSpecificElementsUI.SetActive(true);

        yield return new WaitUntil(() => !visualizerSpecificElementsUI.activeSelf);

        BroadcastSetupValues();
        BroadcastTrackInfo();
    }

    private void BroadcastSetupValues()
    {
        BroadcastTrackInfo();
        BroadcastVisualizerSpecificsElementsInfo();
        BroadcastVisualizerElementsInfo();
        BroadcastVisualizerFloatValues();
        BroadcastVisualizerIntValues();
        BroadcastVisualizerBoolValues();
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

    private Sprite MakeSpriteFromTex(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public IEnumerator BrowseForImageFile(Action<Sprite> onSuccess, Action always)
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpeg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Image from File: " + x);
            LoadImageFromFile(x,
                (filePath, texture) =>
                {
                    Debug.Log("Successfully Loaded Image from path = " + x);
                    onSuccess?.Invoke(MakeSpriteFromTex(texture));
                },
            x =>
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load image from selected file " +
                    "- Ensure the file selected is an appropriate file type");
                Debug.Log("Failed to Load Image from path = " + x);
            });
        }, "Select Image", "Load"));

        always?.Invoke();
    }

    public IEnumerator BrowseForTrackFile(Action<AudioClip> onSuccess, Action always)
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));

        yield return StartCoroutine(BrowseForSingleFile(x =>
        {
            Debug.Log("Attempting to Load Audio from File: " + x);

            StartCoroutine(LoadAudioClipFromFile(x,
                (filePath, clip) =>
                {
                    onSuccess?.Invoke(clip);
                },
                x =>
                {
                    UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load track from selected file " +
                        "- Ensure the file selected is an appropriate file type");
                    Debug.Log("Failed to Load Audio Clip from path = " + x);
                }));
        }, "Select Track", "Load"));

        always?.Invoke();
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
        AudioType audioFileType = AudioType.UNKNOWN;
        try
        {
            audioFileType = GetAudioType(filePath);
        }
        catch (InvalidFileTypeException e)
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Attempted to load an invalid audio file - Supported audio file extensions are " 
                + GetSupportedAudioFileExtensions());
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, audioFileType))
        {
            int loadingKey = UIManager._Instance.AddLoading("Attempting to load audio from file");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load Track");
                Debug.Log("Failed to download Track from path: " + filePath);
                onFailure?.Invoke(filePath);
            }
            else
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Successfully loaded Track");
                Debug.Log("Successfully downloaded Track from path: " + filePath);
                onSuccess?.Invoke(filePath, DownloadHandlerAudioClip.GetContent(www));
            }

            UIManager._Instance.RemoveLoading(loadingKey);
        }
    }

    private string GetSupportedAudioFileExtensions()
    {
        return "[ .wav, .mp3, .ogg ]";
    }

    private void LoadImageFromFile(string filePath, Action<string, Texture2D> onSuccess, Action<string> onFailure)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);

        try
        {
            if (tex.LoadImage(bytes))
            {
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.SUCCESS, "Successfully loaded Image");
                onSuccess?.Invoke(filePath, tex);
            }
            else
            {
                onFailure?.Invoke(filePath);
                UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Failed to load Image");
            }
        }
        catch (Exception e)
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.ERROR, "Attempted to load an invalid image file");
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
        string extension = StringHelper.GetFileExtension(filePath);
        switch (extension)
        {
            case "wav":
                return AudioType.WAV;
            case "mp3":
                return AudioType.MPEG;
            case "ogg":
                return AudioType.OGGVORBIS;
            default:
                throw new InvalidFileTypeException(extension);
        }
    }

    public void AddElementToColorsList()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Color or Gradient?", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("Color", () => AddColorElement()),
            new ActionSelection("Gradient", () => AddGradientElement()),
        }));
    }

    private void AddColorElement()
    {
        trackInfo.Colors.Add(Color.white);
        ColorListElement spawned = Instantiate(colorListElement, colorsList);
        spawned.Set(trackInfo.Colors.Count - 1, Color.white);
    }

    private void AddGradientElement()
    {
        Gradient g = defaultGradient;
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

    public void UpdateSetting(string key, float v)
    {
        if (!visualizerFloatValues.ContainsKey(key))
        {
            RegisterFloatValue(key, v);
        }
        else
        {
            visualizerFloatValues[key] = v;

            switch (key)
            {
                case "SMOOTHING_STRENGTH":
                    smoothingEquationStrength = v;
                    break;
                case "SMOOTHING_SHIFT":
                    smoothingEquationShift = v;
                    break;
                case "SMOOTHING_SCALE":
                    smoothingEquationScale = v;
                    break;
                case "MIN_NORMALIZED_VALUE":
                    minNormalizedSampleValue = v;
                    break;
                case "MAX_NORMALIZED_VALUE":
                    maxNormalizedSampleValue = v;
                    break;
            }
        }

        BroadcastVisualizerFloatValues();
    }

    public void UpdateSetting(string key, int v)
    {
        if (!visualizerIntValues.ContainsKey(key))
        {
            RegisterIntValue(key, v);
        }
        else
        {
            visualizerIntValues[key] = v;
        }

        BroadcastVisualizerIntValues();
    }

    public void UpdateSetting(string key, bool v)
    {
        if (!visualizerBoolValues.ContainsKey(key))
        {
            RegisterBoolValue(key, v);
        }
        else
        {
            visualizerBoolValues[key] = v;

            switch (key)
            {
                case "ENABLE_SMOOTHING":
                    enableSmoothing = v;
                    break;
                case "ENABLE_NORMALIZATION":
                    normalizeSamples = v;
                    break;
            }
        }

        BroadcastVisualizerBoolValues();
    }

    public bool HasFloatSetting(string key)
    {
        return visualizerFloatValues.ContainsKey(key);
    }

    public bool HasIntSetting(string key)
    {
        return visualizerIntValues.ContainsKey(key);
    }

    public bool HasBoolSetting(string key)
    {
        return visualizerBoolValues.ContainsKey(key);
    }

    public float GetFloatSetting(string key)
    {
        return visualizerFloatValues[key];
    }

    public int GetIntSetting(string key)
    {
        return visualizerIntValues[key];
    }

    public bool GetBoolSetting(string key)
    {
        return visualizerBoolValues[key];
    }

    public VisualizerElementsSettings GetVisualizerSpecificElementSettings(string key)
    {
        if (!visualizerSpecificElementsInfo.ContainsKey(key))
        {
            visualizerSpecificElementsInfo.Add(key, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
        }

        return visualizerSpecificElementsInfo[key];
    }

    public void UpdateVisualizerSpecificElementsSettings(string key, VisualizerElementsSettings settings)
    {
        if (!visualizerSpecificElementsInfo.ContainsKey(key))
        {
            RegisterVisualizerSpecificElement(key, settings);
        }
        else
        {
            visualizerSpecificElementsInfo[key] = settings;
        }

        BroadcastVisualizerSpecificsElementsInfo();
    }


    public void RegisterFloatValue(string key, float v)
    {
        visualizerFloatValues.Add(key, v);
        BroadcastVisualizerFloatValues();
    }

    public void RegisterIntValue(string key, int v)
    {
        visualizerIntValues.Add(key, v);
        BroadcastVisualizerIntValues();
    }

    public void RegisterBoolValue(string key, bool v)
    {
        visualizerBoolValues.Add(key, v);
        BroadcastVisualizerIntValues();
    }

    public void RegisterVisualizerSpecificElement(string key)
    {
        RegisterVisualizerSpecificElement(key, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
        BroadcastVisualizerSpecificsElementsInfo();
    }

    public void RegisterVisualizerSpecificElement(string key, VisualizerElementsSettings settings)
    {
        visualizerSpecificElementsInfo.Add(key, settings);
        BroadcastVisualizerSpecificsElementsInfo();
    }

    public VisualizerElementsSettings GetBaseVisualizerElementSettings(VisualizerElementLabel label)
    {
        return baseVisualizerElementsInfo[label];
    }

    public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> GetBaseVisualizerElementSettings()
    {
        return baseVisualizerElementsInfo;
    }

    public void UpdateBaseVisualizerElementsSettings(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> settings)
    {
        baseVisualizerElementsInfo = settings;
        BroadcastVisualizerElementsInfo();
    }

    public void UpdateBaseVisualizerElementSettings(VisualizerElementLabel label, VisualizerElementsSettings newSettings)
    {
        baseVisualizerElementsInfo[label] = newSettings;
        BroadcastVisualizerElementsInfo();
    }

    private Texture2D MakeTexFromSprite(Sprite sprite)
    {
        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private IEnumerator SavePreset(bool savingAudioAndCoverArt)
    {
        yield return StartCoroutine(UIManager._Instance.PopupInputField(trackInfo.Title, "Name your Preset", "Confirm Preset Name", "Cancel", false,
            x =>
            {
                AudioClipData audioClip = new();
                SpriteData coverArt = new();
                if (savingAudioAndCoverArt)
                {
                    if (audioSource.clip != null)
                    {
                        // for saving audio
                        int channels = audioSource.clip.channels;
                        int samples = audioSource.clip.samples;
                        float[] samplesData = new float[samples * channels];
                        audioSource.clip.GetData(samplesData, 0);
                        audioClip = new AudioClipData(channels, audioSource.clip.frequency, samplesData);
                    } else
                    {
                        UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING,
                            "No audio has been loaded to include in preset - No audio will be included in preset");
                    }

                    if (trackInfo.CoverArt != null)
                    {
                        // for saving cover art
                        Texture2D tex = MakeTexFromSprite(trackInfo.CoverArt);
                        coverArt = new SpriteData(tex.EncodeToPNG(), tex.width, tex.height, tex.format);
                    } else
                    {
                        UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, 
                            "No cover art has been loaded to include in preset - No audio will be included in preset");
                    }
                }

                VisualizerPreset preset = new VisualizerPreset(trackInfo.Title, audioClip, coverArt,
                    trackInfo.Colors, trackInfo.Gradients,
                    loadedFontData.Values.ToList(), baseVisualizerElementsInfo, visualizerSpecificElementsInfo,
                    visualizerFloatValues, visualizerIntValues, visualizerBoolValues);

                SaveManager._Instance.SavePreset(x, preset, null, null);
            },
            null));
    }

    [ContextMenu("Save Preset")]
    public void SavePreset()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Include Audio and Cover Art in Preset?", "Cancel", null, 
            new List<ActionSelection>() 
            {
                new ActionSelection("Yes", () => StartCoroutine(SavePreset(true))),
                new ActionSelection("No", () => StartCoroutine(SavePreset(false)))
            }));
    }

    private void SetFromPreset(VisualizerPreset preset)
    {        
        if (preset.Audio.Set)
        {
            // Loading audio
            AudioClipData audioClip = preset.Audio;
            AudioClip clip = AudioClip.Create("AudioClip", audioClip.Samples.Length, audioClip.Channels, audioClip.Frequency, false);
            clip.SetData(audioClip.Samples, 0);
            audioSource.clip = clip;

            trackInfo.AudioClip = clip;
            trackInfo.Duration = StringHelper.GetDurationText(clip.length);
            trackInfo.Title = preset.Title;
        }

        if (preset.CoverArt.Set)
        {
            // Loading cover
            Texture2D texture = new Texture2D(preset.CoverArt.Width, preset.CoverArt.Height, preset.CoverArt.TextureFormat, false);
            texture.LoadImage(preset.CoverArt.PNGEncodedData);
            trackInfo.CoverArt = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        // Set colors
        ClearColorsList();
        trackInfo.Colors = preset.Colors;
        foreach (GradientData gData in preset.Gradients)
        {
            Gradient g = new Gradient();
            g.SetKeys(gData.ColorKeys, gData.AlphaKeys);
            g.mode = gData.Mode;
            trackInfo.Gradients.Add(g);
        }
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

        // Set other visualizer data
        baseVisualizerElementsInfo = preset.BaseVisualizerElements;
        visualizerSpecificElementsInfo = preset.VisualizerSpecificElements;
        visualizerFloatValues = preset.VisualizerFloatValues;
        visualizerIntValues = preset.VisualizerIntValues;
        visualizerBoolValues = preset.VisualizerBoolValues;

        BroadcastSetupValues();
    }

    public void LoadPreset(string filePath, Action<string, VisualizerPreset> onSuccess, Action<string> onFailure)
    {
        SaveManager._Instance.LoadPreset(filePath,
            (filePath, loadedPreset) =>
            {
                SetFromPreset(loadedPreset);
                onSuccess?.Invoke(filePath, loadedPreset);
            }, filePath => onFailure?.Invoke(filePath));
    }

    [ContextMenu("BroadcastTempo")]
    private void BroadcastTempo()
    {
        tempoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTempo>().ToList();

        // Send data out
        tempoListeners.ForEach(item => item.RecieveTempo(estimatedBPM));
    }

    [ContextMenu("BroadcastControlScheme")]
    private void BroadcastControlScheme()
    {
        controlSchemeListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveControlScheme>().ToList();

        // Send data out
        controlSchemeListeners.ForEach(item => item.RecieveControlScheme(activeControlScheme));
    }


    [ContextMenu("BroadcastTrackInfo")]
    private void BroadcastTrackInfo()
    {
        trackInfoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTrackInfo>().ToList();

        UpdateTrackFonts();

        // Send data out
        trackInfoListeners.ForEach(item => item.RecieveTrackInfo(trackInfo));

        SetVisualizerCVActive();
    }

    [ContextMenu("BroadcastVisualizerElementsInfo")]
    private void BroadcastVisualizerElementsInfo()
    {
        // Attempt to find any listeners if the list is empty
        if (visualizerElementsInfoListeners.Count == 0)
        {
            visualizerElementsInfoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerElementsInfo>().ToList();
        }

        // Send data out
        visualizerElementsInfoListeners.ForEach(item => item.RecieveVisualizerElementsInfo(baseVisualizerElementsInfo));

        SetVisualizerCVActive();
    }

    [ContextMenu("BroadcastVisualizerElementsInfo")]
    private void BroadcastVisualizerSpecificsElementsInfo()
    {
        visualizerSpecificElementsInfoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerSpecificElementsInfo>().ToList();

        // Send data out
        visualizerSpecificElementsInfoListeners.ForEach(item => item.RecieveVisualizerSpecificElementsInfo(visualizerSpecificElementsInfo));

        SetVisualizerCVActive();
    }

    [ContextMenu("BroadcastVisualizerFloatValues")]
    private void BroadcastVisualizerFloatValues()
    {
        visualizerFloatValueListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerFloatValues>().ToList();

        // Send data out
        visualizerFloatValueListeners.ForEach(item => item.RecieveVisualizerFloatValues(visualizerFloatValues));
    }

    [ContextMenu("BroadcastVisualizerIntValues")]
    private void BroadcastVisualizerIntValues()
    {
        visualizerIntValueListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerIntValues>().ToList();

        // Send data out
        visualizerIntValueListeners.ForEach(item => item.RecieveVisualizerIntValues(visualizerIntValues));
    }


    [ContextMenu("BroadcastVisualizerBoolValues")]
    private void BroadcastVisualizerBoolValues()
    {
        visualizerBoolValueListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerBoolValues>().ToList();

        // Send data out
        visualizerBoolValueListeners.ForEach(item => item.RecieveVisualizerBoolValues(visualizerBoolValues));
    }

    private void SetVisualizerCVActive()
    {
        if (visualizerCanvasGroup != null)
        {
            visualizerCanvasGroup.alpha = 1;
            visualizerCanvasGroup.blocksRaycasts = true;
        }
    }

    public void SyncTempoListeners()
    {
        BroadcastTempo();
    }

    public void OpenTempoMenu()
    {
        tempoMenuUI.SetActive(true);
    }

    public void CloseTempoMenu()
    {
        tempoMenuUI.SetActive(false);
    }

    public void UpdateEstimatedBPM(string s)
    {
        float v;
        if (float.TryParse(s, out v))
        {
            UpdateEstimatedBPM(v);
        }
    }

    public void UpdateEstimatedBPM(float v)
    {
        estimatedBPM = v;
        BroadcastTempo();
    }


    [ContextMenu("Start Tapping BPM")]
    public void StartTappingBPM()
    {
        StartCoroutine(TapBPM());
    }

    [ContextMenu("Stop Tapping BPM")]
    public void StopTappingBPM()
    {
        tappingBPM = false;
    }

    [ContextMenu("Start Estimating BPM")]
    public void StartEstimateBPM()
    {
        StartCoroutine(EstimateBPM());
    }

    [ContextMenu("Stop Estimating BPM")]
    public void StopEstimatingBPM()
    {
        estimatingBPM = false;
    }

    private void IncrementBeatsLastSecond(float v)
    {
        beatsLastSecond++;
    }

    public void TapButtonPressed()
    {
        if (!tappingBPM)
        {
            StartTappingBPM();
        }
        else
        {
            didTap = true;
        }
    }

    private IEnumerator TapBPM()
    {
        float lastTap = 0;
        float averageSecondsBetweenTaps = 0;
        float timer = 0;
        List<float> secondsBetweenTaps = new();

        tappingBPM = true;
        while (tappingBPM)
        {
            if (estimatingBPM)
            {
                yield break;
            }

            timer += Time.deltaTime;

            // space or left mosue button to tap
            if (didTap)
            {
                didTap = false;

                // find the time since the last tap and add it to the list
                secondsBetweenTaps.Add(Mathf.Abs(timer - lastTap));

                if (secondsBetweenTaps.Count > tapBufferCount)
                {
                    secondsBetweenTaps.RemoveAt(0);
                }

                // find average
                foreach (float f in secondsBetweenTaps)
                {
                    averageSecondsBetweenTaps += f;
                }
                averageSecondsBetweenTaps /= secondsBetweenTaps.Count;

                // calculate
                estimatedBPM = 60 / averageSecondsBetweenTaps; // minutes conversion

                SetTapperInputFieldText();

                // update last tap time
                lastTap = timer;
            }

            yield return null;
        }

        BroadcastTempo();
    }

    private IEnumerator EstimateBPM()
    {
        beatsLastSecond = 0;
        beatCountsPerSecondBuffer.Clear();

        tempoTapperInputField.interactable = false;
        tempoTapperButton.interactable = false;

        OnEnergySpike += IncrementBeatsLastSecond;
        estimatingBPM = true;

        while (estimatingBPM)
        {
            // wait a second
            yield return new WaitForSeconds(1);

            // add new value for however many beats occurred since last iteration
            beatCountsPerSecondBuffer.Add(beatsLastSecond);

            // reset counter
            beatsLastSecond = 0;

            // remove old value if neccessary
            if (beatCountsPerSecondBuffer.Count > measureBPMOverInterval)
            {
                beatCountsPerSecondBuffer.RemoveAt(0);
            }

            // calculate bpm
            float beatsOverLastInterval = 0;
            foreach (int value in beatCountsPerSecondBuffer) { beatsOverLastInterval += value; }
            estimatedBPM = beatsOverLastInterval * (60 / measureBPMOverInterval);

            SetTapperInputFieldText();
        }

        tempoTapperInputField.interactable = true;
        tempoTapperButton.interactable = true;

        OnEnergySpike -= IncrementBeatsLastSecond;

        BroadcastTempo();
    }

    private void SetTapperInputFieldText()
    {
        tempoTapperInputField.text = Math.Round(estimatedBPM, 2).ToString();
    }

    public void SetLightColorToVisualizerSpecificElementSettings(string key, Light light)
    {
        if (!visualizerSpecificElementsInfo.ContainsKey(key)) return;
        light.color = GetColor(visualizerSpecificElementsInfo[key].ColorType, visualizerSpecificElementsInfo[key].ColorIndex);
    }

    public void SelectControlScheme(ControlScheme newScheme)
    {
        activeControlScheme = newScheme;

        escapeMenu.UpdateDisplayedControls(newScheme);

        BroadcastControlScheme();
    }

    public void EditTrackTitle()
    {
        StartCoroutine(UIManager._Instance.PopupInputField(VisualizerManager._Instance.TrackTitle, "Enter a new Title", "Accept", "Cancel", false,
            x => VisualizerManager._Instance.UpdateTrackTitle(x), null));
    }
}