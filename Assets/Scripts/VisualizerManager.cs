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

public enum ImportableAudioSource
{
    YOUTUBE,
    SOUNDCLOUD
}

public enum AudioChannel
{
    STEREO,
    LEFT,
    RIGHT
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
    [SerializeField] private TrackInfo trackInfo;
    [SerializeField] private TMP_FontAsset defaultFont;

    private Dictionary<string, int> loadedFontIndices = new();
    private Dictionary<string, FontFileData> loadedFontData = new();
    private Dictionary<string, TMP_FontAsset> loadedTMPFontAssets = new();

    [Header("Other Settings")]
    [SerializeField] private List<string> visualizerSpecificElementKeys = new();
    [SerializeField] private List<SerializableKeyValuePair<string, float>> visualizerFloatValueKeys = new();
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
    [SerializeField] private float sampleMultiplier = 1;
    private int numSamples = 512;

    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    [Header("Smoothing")]
    [SerializeField] private bool smoothLowToHigh;
    [SerializeField] private float smoothingEquationStrength = .5f;
    [SerializeField] private float smoothingEquationShift = 1;
    [SerializeField] private float smoothingEquationScale = 128;
    private float smoothingValue;

    [Header("Normalizing")]
    [SerializeField] private bool normalizeSamples;
    [SerializeField] private float minSampleValue = 0;
    [SerializeField] private float maxSampleValue = 1;
    private float smallestValueLeft = Mathf.Infinity;
    private float largestValueLeft = 0;
    private float smallestValueRight = Mathf.Infinity;
    private float largestValueRight = 0;

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

    [Header("Beat Detection Settings")]
    [SerializeField] private float ampSpikeDetectionSensitivity = 0.9f;

    [Header("Scenarios")]
    [SerializeField] private List<SerializableKeyValuePair<string, GameObject>> scenarios = new();
    [SerializeField] private GameObject defaultScenary;
    [SerializeField] private GameObject scenarioSelection;

    [Header("Transition Settings")]
    [SerializeField] private TransitionData initialTransition;

    [Header("References")]
    [SerializeField] private CanvasGroup visualizerCanvasGroup;
    [SerializeField] private GameObject visualizerElementsUI;
    [SerializeField] private GameObject tempoMenuUI;
    [SerializeField] private TMP_InputField tempoTapperInputField;
    [SerializeField] private Button tempoTapperButton;
    [SerializeField] private GameObject colorsUI;
    [SerializeField] private GameObject fontsUI;
    [SerializeField] private Transform colorsList;
    [SerializeField] private Transform fontsList;
    private AudioSource audioSource;
    private UniversalAdditionalCameraData activeCameraAdditionalCameraData;
    private Volume volume;

    [Header("Prefabs")]
    [SerializeField] private ColorListElement colorListElement;
    [SerializeField] private GradientListElement gradientListElement;
    [SerializeField] private FontListElement fontListElement;

    private List<IRecieveActiveCamera> activeCameraListeners = new();
    private List<IRecieveTrackInfo> trackInfoListeners = new();
    private List<IRecieveTempo> tempoListeners = new();
    private List<IRecieveVisualizerElementsInfo> visualizerElements = new();
    private List<IRecieveVisualizerSpecificElementsInfo> visualizerSpecificElements = new();
    private List<IRecieveVisualizerFloatValues> visualizerFloatValueListeners = new();
    private Dictionary<VisualizerElementLabel, VisualizerElementsSettings> visualizerElementsInfo = new();
    private Dictionary<string, VisualizerElementsSettings> visualizerSpecificElementsInfo = new();
    private Dictionary<string, float> visualizerFloatValues = new();

    private bool hasSongStarted = false;
    private float lastAudioSourceTime;

    // Events
    public Action OnSongEnd;
    public Action OnSongStart;
    public Action<float> OnAmplitudeSpike;
    public Action<float> OnBeat;

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

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        // Get audio source component
        audioSource = GetComponent<AudioSource>();
        volume = FindObjectOfType<Volume>();

        PopulateColorsList();
        PopulateFontsList();

        // populate dictionaries
        foreach (VisualizerElementLabel item in Enum.GetValues(typeof(VisualizerElementLabel)))
        {
            visualizerElementsInfo.Add(item, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
        }
        foreach (string key in visualizerSpecificElementKeys)
        {
            visualizerSpecificElementsInfo.Add(key, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
        }
        foreach (SerializableKeyValuePair<string, float> kvp in visualizerFloatValueKeys)
        {
            visualizerFloatValues.Add(kvp.Key, kvp.Value);
        }
    }

    private void Start()
    {
        // only 1 scenario, we'd just go to track selection
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

        if (audioSource.isPlaying)
        {
            // Spectrum Data
            GetSpectrumAudioSource();

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

        // check for beat
        if (currentEnergy > averageLocalEnergy * varianceSensitivity) 
        { 
            OnBeat?.Invoke(currentEnergy);
        }
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

    [ContextMenu("Reset Sample Normalization Values")]
    private void ResetSampleNormalizationValues()
    {
        smallestValueLeft = Mathf.Infinity;
        largestValueLeft = 0;
        smallestValueRight = Mathf.Infinity;
        largestValueRight = 0;
    }

    private void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);

        // reset values for normalizing
        ResetSampleNormalizationValues();

        // initial pass
        for (int i = 0; i < numSamples; ++i)
        {
            // smooth
            if (smoothLowToHigh)
            {
                smoothingValue = GetSmoothingValue(i, numSamples);
                leftAudioSamples[i] = leftAudioSamples[i] * smoothingValue;
                rightAudioSamples[i] = rightAudioSamples[i] * smoothingValue;
            }

            if (normalizeSamples)
            {
                if (leftAudioSamples[i] < smallestValueLeft) smallestValueLeft = leftAudioSamples[i];
                if (leftAudioSamples[i] > largestValueLeft) largestValueLeft = leftAudioSamples[i];
                if (rightAudioSamples[i] < smallestValueRight) smallestValueRight = rightAudioSamples[i];
                if (rightAudioSamples[i] > largestValueRight) largestValueRight = rightAudioSamples[i];
            }
        }

        // secondary pass
        for (int i = 0; i < numSamples; ++i)
        {
            if (normalizeSamples)
            {
                leftAudioSamples[i] = MathHelper.Normalize(leftAudioSamples[i], smallestValueLeft, largestValueLeft, minSampleValue, maxSampleValue);
                rightAudioSamples[i] = MathHelper.Normalize(rightAudioSamples[i], smallestValueRight, largestValueRight, minSampleValue, maxSampleValue);
            }

            // apply final multiplier
            leftAudioSamples[i] *= sampleMultiplier;
            rightAudioSamples[i] *= sampleMultiplier;
        }
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
        if (amplitude > ampSpikeThreshold) OnAmplitudeSpike?.Invoke(amplitude - ampSpikeThreshold);
    }

    private void CreateAudioProfile()
    {
        for (int i = 0; i < highestValuePerFrequencyBand.Length; i++)
            highestValuePerFrequencyBand[i] = beginningHighestFrequencyBandValue;
    }

    public void BeginPlayback()
    {
        if (startSongAtSeconds > audioSource.clip.length)
            Debug.LogWarning("Attempted to start the track at a position longer than the track itself");

        // Set the point where the AudioSource begins
        audioSource.time = startSongAtSeconds;

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

    public void ChangeVolume(float amount)
    {
        audioSource.volume += amount;
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
        yield return UIManager._Instance.PopupActionSelection("Load Track from URL or File?", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("URL", null, PopoutEnterTrackURL()),
            new ActionSelection("File", null, BrowseForTrack())
        });
    }

    private IEnumerator PopoutEnterTrackURL()
    {
        yield return UIManager._Instance.PopupInputField("Track URL", "Enter YouTube or Soundcloud URL", "Confirm", "Cancel", false,
                    AttemptToDownloadTrackFromURL, OnFailDownloadTrackFromURL());
    }

    private IEnumerator OnFailDownloadTrackFromURL()
    {
        UIManager._Instance.AddNewMessage("Failed to download Track from URL");
        yield return null;
    }

    private IEnumerator AttemptToDownloadTrackFromURL(string url)
    {
        UIManager._Instance.AddNewMessage("Attempting to Download Track from: " + url);

        ImportableAudioSource origin = GetAudioFileOrigin(url);
        switch (origin)
        {
            case ImportableAudioSource.SOUNDCLOUD:
                yield return StartCoroutine(DownloadTrackFromSoundCloud(url, false,
                    (url, clip) =>
                    {
                        SetTrack(url, clip);
                    }, null));
                break;
            case ImportableAudioSource.YOUTUBE:
                yield return StartCoroutine(DownloadTrackFromYouTube(url,
                    (url, clip) =>
                    {
                        SetTrack(url, clip);
                    }, null));
                break;
            default:
                Debug.Log("Unsupported Origin: " + origin);
                break;
        }
    }

    private ImportableAudioSource GetAudioFileOrigin(string url)
    {
        url = url.Split("https://")[1];
        url = url.Split('/')[0].ToLower();

        if (url.Contains("soundcloud"))
            return ImportableAudioSource.SOUNDCLOUD;
        else if (url.Contains("youtube"))
            return ImportableAudioSource.YOUTUBE;
        else
        {
            throw new Exception(); // TODO: Custom Exceptions
        }
    }

    private ValueTask<string[]> YouTubeToMP3(string mediaUrl, string directoryPath)
    {
        YouTube youtube = YouTube.Default;
        YouTubeVideo vid = youtube.GetVideo(mediaUrl);

        // determine mp4 path
        string inputFilePath = Path.Combine(directoryPath, vid.Title);
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
        }

        return new ValueTask<string[]>(new string[] { inputFilePath, outputFilePath });
    }

    private async void DownloadTrackFromYouTubeAsync(string mediaUrl, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        // create folder if neccessary
        string directoryPath = Path.Combine(Application.dataPath, "../temp");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // add UI
        UIManager._Instance.AddLoading("Downloading from: " + mediaUrl);

        var task = Task.Run(async () => await YouTubeToMP3(mediaUrl, directoryPath));
        await task;

        // add UI
        UIManager._Instance.RemoveLoading("Downloading from: " + mediaUrl);

        string mp4FilePath = task.Result[0];
        string mp3FilePath = task.Result[1];

        // delete input file
        File.Delete(mp4FilePath);

        // Delete output files
        onSuccess += (path, clip) => File.Delete(mp3FilePath);
        onFailure += path => File.Delete(mp3FilePath);

        // load data from output file
        StartCoroutine(LoadAudioClipFromFile(mp3FilePath, onSuccess, onFailure));
    }

    private IEnumerator DownloadTrackFromYouTube(string mediaUrl, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        DownloadTrackFromYouTubeAsync(mediaUrl, onSuccess, onFailure);
        yield return null;
    }

    private IEnumerator DownloadTrackFromYouTubeWait(string mediaUrl, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        bool completed = false;
        onSuccess += (filePath, clip) => completed = true;
        onFailure += filePath => completed = true;
        DownloadTrackFromYouTubeAsync(mediaUrl, onSuccess, onFailure);
        yield return new WaitUntil(() => completed);
    }

    private async void DownloadTrackFromSoundCloudAsync(string mediaUrl, bool useCover, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        SoundCloudClient soundcloud = new SoundCloudClient();

        // add UI
        UIManager._Instance.AddLoading("Fetching from: " + mediaUrl);

        // setup fetch
        var getTask = Task.Run(async () => await soundcloud.Tracks.GetAsync(mediaUrl));

        await getTask;

        // remove UI
        UIManager._Instance.RemoveLoading("Fetching from: " + mediaUrl);

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
        UIManager._Instance.AddLoading("Downloading from: " + mediaUrl);

        // download the data
        var downloadTask = Task.Run(async () => await soundcloud.DownloadAsync(track, outputFilePath));

        await downloadTask;

        // remove UI
        UIManager._Instance.RemoveLoading("Downloading from: " + mediaUrl);

        if (useCover)
        {
            // load cover art on success as well
            onSuccess += (path, clip) => StartCoroutine(AttemptToDownloadImageFromURL(track.ArtworkUrl.ToString()));
        }

        StartCoroutine(LoadAudioClipFromFile(outputFilePath, onSuccess, onFailure));

        // delete file
        File.Delete(outputFilePath);
    }

    private IEnumerator DownloadTrackFromSoundCloud(string mediaUrl, bool useCover, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        DownloadTrackFromSoundCloudAsync(mediaUrl, useCover, onSuccess, onFailure);
        yield return null;
    }

    private IEnumerator DownloadTrackFromSoundCloudWait(string mediaUrl, bool useCover, Action<string, AudioClip> onSuccess, Action<string> onFailure)
    {
        bool completed = false;
        onSuccess += (filePath, clip) => completed = true;
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
                x => Debug.Log("Failed to Load Audio Clip from path = " + x)));
        }, "Select Track", "Load"));
    }

    private void SetTrack(string filePath, AudioClip clip)
    {
        Debug.Log("Successfully Loaded Audio Clip from = " + filePath);
        audioSource.clip = clip;
        trackInfo.Title = StringHelper.GetFileName(filePath);
        trackInfo.Duration = StringHelper.GetDurationText(audioSource.clip.length);

        BroadcastTrackInfo();
    }

    #endregion

    #region Cover Art Selection
    public IEnumerator RunCoverArtSelection()
    {
        yield return UIManager._Instance.PopupActionSelection("Load Art from URL or File?", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("URL", null, PopoutEnterImageURL()),
            new ActionSelection("File", null, BrowseForImage())
        });
    }

    private IEnumerator PopoutEnterImageURL()
    {
        yield return UIManager._Instance.PopupInputField("Image URL", "Enter Image URL", "Confirm", "Cancel", false,
                    AttemptToDownloadImageFromURL, OnFailDownloadImageFromURL());
    }

    private IEnumerator OnFailDownloadImageFromURL()
    {
        UIManager._Instance.AddNewMessage("Failed to download Image from URL");
        yield return null;
    }

    private IEnumerator AttemptToDownloadImageFromURL(string url)
    {
        UIManager._Instance.AddLoading("Downloading Image from: " + url);

        yield return StartCoroutine(DownloadImage(url,
                (url, tex) =>
                {
                    UIManager._Instance.AddNewMessage("Successfully downloaded image from: " + url);
                    Sprite downloadedSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100f);
                    trackInfo.CoverArt = downloadedSprite;
                    BroadcastTrackInfo();

                    UIManager._Instance.RemoveLoading("Downloading Image from: " + url);
                },
                url =>
                {
                    UIManager._Instance.AddNewMessage("Failed to download image from: " + url);
                    UIManager._Instance.RemoveLoading("Downloading Image from: " + url);
                }));
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
                    trackInfo.CoverArt = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    BroadcastTrackInfo();
                },
            x => Debug.Log("Failed to Load Image from path = " + x));
        }, "Select Cover Art", "Load"));
    }

    private IEnumerator DownloadImage(string mediaUrl, Action<string, Texture2D> onSuccess, Action<string> onFailure)
    {
        UIManager._Instance.AddNewMessage("Attempting to Download Image from: " + mediaUrl);

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

    public IEnumerator RunVisualizerElementsSelection()
    {
        // Enable UI
        visualizerElementsUI.SetActive(true);

        BroadcastVisualizerElementsInfo();
        BroadcastVisualizerSpecificsElementsInfo();
        BroadcastVisualizerFloatValues();

        yield return new WaitUntil(() => !visualizerElementsUI.activeSelf);

        BroadcastVisualizerElementsInfo();
        BroadcastVisualizerSpecificsElementsInfo();
        BroadcastVisualizerFloatValues();
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
                Debug.Log("Failed to download Track from path: " + filePath);
                UIManager._Instance.AddNewMessage("Failed to download Track from path: " + filePath);
                onFailure?.Invoke(filePath);
            }
            else
            {
                Debug.Log("Successfully downloaded Track from path: " + filePath);
                UIManager._Instance.AddNewMessage("Successfully downloaded Track from path: " + filePath);
                onSuccess?.Invoke(filePath, DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }

    private void LoadImageFromFile(string filePath, Action<string, Texture2D> onSuccess, Action<string> onFailure)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        if (tex.LoadImage(bytes))
        {
            onSuccess?.Invoke(filePath, tex);
        } else
        {
            onFailure?.Invoke(filePath);
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

    public void UpdateFloatSetting(string key, float v)
    {

    }

    public float GetFloatSetting(string key)
    {
        return visualizerFloatValues[key];
    }

    public VisualizerElementsSettings GetVisualizerSpecificElementSettings(string specificElementKey)
    {
        return visualizerSpecificElementsInfo[specificElementKey];
    }

    public VisualizerElementsSettings GetBaseVisualizerElementSettings(VisualizerElementLabel label)
    {
        return visualizerElementsInfo[label];
    }

    public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> GetVisualizerElementSettings()
    {
        return visualizerElementsInfo;
    }

    public void SetVisualizerElementsSettings(string key, float v)
    {
        if (!visualizerFloatValues.ContainsKey(key))
        {
            visualizerFloatValues.Add(key, v);
        }
        else
        {
            visualizerFloatValues[key] = v;
        }
        throw new NotImplementedException();
    }

    public void SetVisualizerSpecificElementsSettings(string key, VisualizerElementsSettings settings)
    {
        if (!visualizerSpecificElementsInfo.ContainsKey(key))
        {
            visualizerSpecificElementsInfo.Add(key, settings);
        }
        else
        {
            visualizerSpecificElementsInfo[key] = settings;
        }
        BroadcastVisualizerSpecificsElementsInfo();
    }

    public void SetBaseVisualizerElementsSettings(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> settings)
    {
        visualizerElementsInfo = settings;
        BroadcastVisualizerElementsInfo();
    }

    public void UpdateBaseVisualizerElementSettings(VisualizerElementLabel label, VisualizerElementsSettings newSettings)
    {
        visualizerElementsInfo[label] = newSettings;
        BroadcastVisualizerElementsInfo();
    }

    [ContextMenu("Save Preset")]
    public void SavePreset()
    {
        VisualizerPreset preset = new VisualizerPreset(trackInfo.Colors, trackInfo.Gradients,
            loadedFontData.Values.ToList(), visualizerElementsInfo);

        StartCoroutine(UIManager._Instance.PopupInputField(trackInfo.Title, "Name your Preset", "Confirm Preset Name", "Cancel", false,
            x =>
            {
                string path = SaveManager._Instance.SavePreset(x, preset);
                UIManager._Instance.AddNewMessage("Preset saved to " + path);
            },
            null));
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
            BroadcastVisualizerSpecificsElementsInfo();
            BroadcastVisualizerFloatValues();

            onSuccess(filePath, preset);
        } catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            onFailure(filePath);
        }
    }

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }
    public float GetEstimatedBPM() { return estimatedBPM; }

    [ContextMenu("BroadcastActiveCamera")]
    private void BroadcastActiveCamera()
    {
        activeCameraListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveActiveCamera>().ToList();

        // Send data out
        activeCameraListeners.ForEach(item => item.RecieveActiveCamera(Camera.main));
    }

    [ContextMenu("BroadcastTempo")]
    private void BroadcastTempo()
    {
        tempoListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTempo>().ToList();

        // Send data out
        tempoListeners.ForEach(item => item.RecieveTempo(estimatedBPM));
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
        if (visualizerElements.Count == 0)
        {
            visualizerElements = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerElementsInfo>().ToList();
        }

        // Send data out
        visualizerElements.ForEach(item => item.RecieveVisualizerElementsInfo(visualizerElementsInfo));
    }

    [ContextMenu("BroadcastVisualizerElementsInfo")]
    private void BroadcastVisualizerSpecificsElementsInfo()
    {
        visualizerSpecificElements = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerSpecificElementsInfo>().ToList();

        // Send data out
        visualizerSpecificElements.ForEach(item => item.RecieveVisualizerSpecificElementsInfo(visualizerSpecificElementsInfo));
    }

    [ContextMenu("BroadcastVisualizerFloatValues")]
    private void BroadcastVisualizerFloatValues()
    {
        visualizerFloatValueListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveVisualizerFloatValues>().ToList();

        // Send data out
        visualizerFloatValueListeners.ForEach(item => item.RecieveVisualizerFloatValues(visualizerFloatValues));
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

        OnBeat += IncrementBeatsLastSecond;
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

        OnBeat -= IncrementBeatsLastSecond;

        BroadcastTempo();
    }

    private void SetTapperInputFieldText()
    {
        tempoTapperInputField.text = Math.Round(estimatedBPM, 2).ToString();
    }

    public void SetLightColorToVisualizerSpecificElementSettings(string key, Light light)
    {
        light.color = GetColor(visualizerSpecificElementsInfo[key].ColorType, visualizerSpecificElementsInfo[key].ColorIndex);
    }
}
