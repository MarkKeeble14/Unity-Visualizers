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
using static System.Net.Mime.MediaTypeNames;

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
    [SerializeField] private AudioClip track;
    [SerializeField] private string trackName;
    [SerializeField] private Sprite trackArt;
    [SerializeField] private List<Color> trackColors = new();
    [SerializeField] private List<Gradient> trackGradients = new();

    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private float audioSampleSmoothing = 100;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;

    [Header("Other Settings")]
    [SerializeField] private bool startImmedietelyUponLoadingTrack;
    [SerializeField] private bool runSetup = true;
    [SerializeField] private bool askForTrack = true;
    [SerializeField] private bool askForCoverArt = true;
    [SerializeField] private bool askForColors = true;
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
    [SerializeField] private UniversalRendererData urpData;
    private KawaseBlur kawaseBlurPass;

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

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PopulateColorsList();

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

    [ContextMenu("RebroadcastTrackInfo")]
    private void BroadcastTrackInfo()
    {
        //
        TrackInfo trackInfo = new TrackInfo(trackName, trackArt, StringHelper.GetDurationText(audioSource.clip.length));
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

        StartCoroutine(RunSetup());
    }

    private IEnumerator RunSetup()
    {
        if (askForTrack)
            yield return StartCoroutine(RunTrackSelection());

        if (askForCoverArt)
            yield return StartCoroutine(RunCoverArtSelection());

        if (askForColors)
            yield return StartCoroutine(RunColorsSelection());

        SetupComplete();
    }

    private IEnumerator RunTrackSelection()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));

        yield return StartCoroutine(ShowLoadDialogCoroutine(x =>
        {
            Debug.Log("Attempting to Load Audio from File: " + x);

            StartCoroutine(LoadAudioClipFromFile(x, 
                (filePath, clip) =>
                {
                    Debug.Log("Successfully Loaded Audio Clip from path = " + x);

                    audioSource.clip = clip;
                    trackName = GetFileName(filePath);

                }, 
                x => Debug.Log("Failed to Load Audio Clip from path = " + x)));
        }, "Select Track", "Load"));
    }

    private IEnumerator RunCoverArtSelection()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpeg"));

        yield return StartCoroutine(ShowLoadDialogCoroutine(x =>
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
    }

    private IEnumerator ShowLoadDialogCoroutine(Action<string> toDoWithFile, string dialogTitle, string loadButtonText)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders,
            false, null, null, dialogTitle, loadButtonText);

        if (FileBrowser.Success)
            OnFileSucessfullySelected(FileBrowser.Result[0], toDoWithFile);
        else
            OnFailureToSelectFiles();
    }

    private void OnFileSucessfullySelected(string filePath, Action<string> toDoWithFile)
    {
        toDoWithFile(filePath);
    }

    private void OnFailureToSelectFiles()
    {
        throw new Exception(); // TODO: Custom Exception
    }

    private IEnumerator LoadAudioClipFromFile(string filePath, Action<string, AudioClip> onSucces, Action<string> onFailure)
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
                onSucces(filePath, DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }

    private void LoadImageFromFile(string filePath, Action<string, Texture2D> onSucces, Action<string> onFailure)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        if (tex.LoadImage(bytes))
        {
            onSucces(filePath, tex);
        } else
        {
            onFailure(filePath);
        }
    }

    private string GetFileExtension(string filePath)
    {
        string extension = "";
        for (int i = filePath.Length - 1; i > 0; --i)
        {
            if (filePath[i].Equals('.'))
                break;
            extension += filePath[i];
        }
        return StringHelper.Reverse(extension);
    }

    private string GetFileName(string filePath)
    {
        string pathWithoutExtension = filePath.Split(GetFileExtension(filePath))[0];
        string fileName = "";
        for (int i = pathWithoutExtension.Length - 2; i >= 0; --i)
        {
            char c = pathWithoutExtension[i];
            if (c.Equals('\\'))
                break;
            fileName += c;
        }
        return StringHelper.Reverse(fileName);
    }

    private AudioType GetAudioType(string filePath)
    {
        switch (GetFileExtension(filePath))
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

    private IEnumerator RunColorsSelection()
    {
        // Enable UI

        colorsUI.SetActive(true);

        yield return new WaitUntil(() => !colorsUI.activeSelf);
    }

    [SerializeField] private ColorListElement colorListElement;
    [SerializeField] private Transform colorsList;
    [SerializeField] private GameObject colorsUI;

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
        trackColors[index] = c;
    }
}

