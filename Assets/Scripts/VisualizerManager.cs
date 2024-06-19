using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

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
    private bool hasSongStarted = false;
    private float lastAudioSourceTime;

    [Header("Recording Settings")]
    [SerializeField] private float afterTrackRecordingBufferTime = 10f;

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
            float playthroughPercent = audioSource.time / audioSource.clip.length;
            if (audioSource.time == 0) playthroughPercent = 1;
            return playthroughPercent;
        }
    }

    public float SecondsPlayed
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
    }

    // Start is called before the first frame update
    private void Start()
    {
        CreateAudioProfile();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = track;

        BroadcastTrackInfo();

        if (startByDefault)
        {
            BeginPlayback();
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
        TrackInfo trackInfo = new TrackInfo(trackName, trackArt, GetEndTimeText(track.length));
        FindObjectsOfType<MonoBehaviour>(true).OfType<IRecieveTrackInfo>().ToList().ForEach(item => item.RecieveTrackInfo(trackInfo));
    }

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }

    private string GetEndTimeText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time - (minutes * 60));
        return minutes + ":" + seconds;
    }

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
}

