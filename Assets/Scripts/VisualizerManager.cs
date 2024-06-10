using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Linq;

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

    public Color GetTrackColor(int index) { return trackColors[index]; }
    public Gradient GetTrackGradient(int index) { return trackGradients[index]; }

    private float[] audioSamples = new float[512];
    public float[] AudioSamples => audioSamples;
    public float ZeroSample { get; private set; }

    private float[] frequencyBands = new float[8];
    private float[] bandBuffer = new float[8];
    private float[] bandBufferDecrease = new float[8];

    private AudioSource audioSource;

    [Header("Audio Sampling Settings")]
    [SerializeField] private float audioSampleSmoothing = 100;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;

    [Header("Other Settings")]
    [SerializeField] private bool startByDefault;

    public float PlaythroughPercent
    {
        get
        {
            float playthroughPercent = audioSource.time / audioSource.clip.length;
            if (audioSource.time == 0) playthroughPercent = 1;
            return playthroughPercent;
        }
    }

    private string GetEndTimeText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time - (minutes * 60));
        return minutes + ":" + seconds;
    }

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);

        _Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = track;

        BroadcastTrackInfo();

        if (startByDefault)
        {
            if (Fader._Instance != null)
                Fader._Instance.FadeOutBlocker();
            BeginPlayback();
        }
    }

    public void BeginPlayback()
    {
        // 
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

    // Update is called once per frame
    private void Update()
    {
        // Spectrum Data
        GetSpectrumAudioSource();

        if (audioSamples != null && audioSamples.Length > 0)
        {
            ZeroSample = audioSamples[0] * audioSampleSmoothing;
        }

        // Make Frequency Bands
        MakeFrequencyBands();

        // Band Buffer
        CalcBandBuffer();
    }

    public float GetBandValue(int band, bool useBuffer) { return useBuffer ? bandBuffer[band] : frequencyBands[band]; }


    private void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.Blackman);
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
                average += audioSamples[count] * (count + 1);
                count++;
            }

            average /= count;

            frequencyBands[i] = average * 10;
        }
    }

    private void CalcBandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (frequencyBands[g] > bandBuffer[g])
            {
                bandBuffer[g] = frequencyBands[g];
                bandBufferDecrease[g] = defaultBandBufferDecrease;
            }

            if (frequencyBands[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bandBufferDecrease[g];
                bandBufferDecrease[g] *= bandBufferDecreaseMultPerFrame;
            }
        }
    }
}
