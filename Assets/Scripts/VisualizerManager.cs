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
    [SerializeField] private CanvasGroup visualizerCanvasGroup;

    [Header("Track Info")]
    [SerializeField] private AudioClip track;
    [SerializeField] private string trackName;
    [SerializeField] private Sprite trackArt;
    [SerializeField] private Color[] trackColors;
    [SerializeField] private Gradient[] trackGradients;

    private float[] leftAudioSamples = new float[512];
    private float[] rightAudioSamples = new float[512];

    // Audio Data
    private float[] frequencyBands = new float[8];
    private float[] frequencyBandBuffer = new float[8];
    private float[] frequencyBandBufferDecrease = new float[8];
    private float[] highestValuePerFrequencyBand = new float[8];
    private float[] audioBands = new float[8];
    private float[] audioBandsBuffer = new float[8];

    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    private float amplitude;
    private float amplitudeBuffer;
    private float highestAmplitude;

    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private float audioSampleSmoothing = 100;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;

    [Header("Other Settings")]
    [SerializeField] private bool startByDefault;
    [SerializeField] private float startSongAtSeconds;
    [SerializeField] private TransitionData initialTransition;
    private bool hasSongStarted = false;
    private float lastAudioSourceTime;

    [Header("Recording Settings")]
    [SerializeField] private float afterTrackRecordingBufferTime = 10f;

    [Header("Blur Settings")]
    [SerializeField] private KawaseBlurSettings blurSettings;

    [Header("References")]
    [SerializeField] private UniversalRendererData urpData;
    private KawaseBlur kawaseBlurPass;

    // Events
    public Action OnSongEnd;
    public Action OnSongStart;

    // References
    private AudioSource audioSource;

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

    public bool IsPlaybackPaused { get; internal set; }

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Only 1 scenario, we'd just go to track selection
        if (scenarios.Count == 0)
        {
            TrackSelection();
        }
    }

    private void TrackSelection()
    {
        SetLoadAudioFilters();
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    private void SetLoadAudioFilters()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        //FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        //FileBrowser.SetDefaultFilter(".mp3");
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders,
            false, null, null, "Select Files", "Load");

        // Dialog is closed
        // Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result); // FileBrowser.Result is null, if FileBrowser.Success is false
    }

    void OnFilesSelected(string[] filePaths)
    {
        // Print paths of the selected files
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        TrackSelected();
    }

    private void TrackSelected()
    {
        // Set canvas group alpha
        visualizerCanvasGroup.alpha = 1;

        // Create audio profile
        CreateAudioProfile();

        // Set track
        audioSource.clip = track;

        // Inform listeners of track
        BroadcastTrackInfo();

        if (startByDefault)
        {
            BeginPlayback();
        }

        // Fetch Kawase Blur Feature
        kawaseBlurPass = (KawaseBlur)urpData.rendererFeatures[0];

        // Change Settings
        SetKawaseBlurFeatureSettings();

        // Play initial transition if there is one
        if (initialTransition.transition != null)
            initialTransition.transition.InitiateTransition(initialTransition.direction);
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

    [ContextMenu("RebroadcastTrackInfo")]
    private void BroadcastTrackInfo()
    {
        //
        TrackInfo trackInfo = new TrackInfo(trackName, trackArt, StringHelper.GetDurationText(track.length));
        FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTrackInfo>().ToList().ForEach(item => item.RecieveTrackInfo(trackInfo));
    }

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }

    public Color GetTrackColor(int index) { return trackColors[index]; }
    public Gradient GetTrackGradient(int index) { return trackGradients[index]; }

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

        TrackSelection();
    }
}

