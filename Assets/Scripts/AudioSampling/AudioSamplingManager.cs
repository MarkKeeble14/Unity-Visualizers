using UnityEngine;
using System;

public class AudioSamplingManager : MonoBehaviour
{
    public static AudioSamplingManager _Instance { get; private set; }

    private float[] leftAudioSamples = new float[512];
    private float[] rightAudioSamples = new float[512];
    private float[] combinedAudioSamples = new float[512];

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

    [Header("Audio Sampling Settings")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private float defaultBandBufferDecrease = 0.005f;
    [SerializeField] private float bandBufferDecreaseMultPerFrame = 1.2f;
    [SerializeField] private float beginningHighestFrequencyBandValue = 5;
    private int numSamples = 512;
    private float samplingLoudnessMultiplier;
    private bool smoothSamples;
    private float smoothingShift;
    private float smoothingScale;
    private float cachedSmoothingValue;

    [Header("Normalizing")]
    [SerializeField] private bool normalizeSamples;
    [SerializeField] private float minNormalizedSampleValue = 0;
    [SerializeField] private float maxNormalizedSampleValue = 1;
    private float cachedSmallestValueLeft = Mathf.Infinity;
    private float cachedLargestValueLeft = 0;
    private float cachedSmallestValueRight = Mathf.Infinity;
    private float cachedLargestValueRight = 0;

    [Header("BPM")]
    [SerializeField] private Vector2Int estimatedBPMRange = new Vector2Int(60, 180);
    private int bpm;
    public int BPM => bpm;
    private float timeSinceLastBeat;
    private float timeBetweenBeats;

    [Header("Beat Detection Settings")]
    [SerializeField] private float ampSpikeDetectionSensitivity = 0.9f;
    public Action<float> OnNewAmplitudePeak;
    public Action OnBeat;

    // Unimplemented
    public Action<float> OnSnareHit;
    public Action<float> OnEnergySpike;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    // Properties
    public float HighestAmplitude => highestAmplitude;
    public float AverageAmplitude { get { return amplitude / highestAmplitude; } }
    public float AverageAmplitudeBuffer { get { return amplitudeBuffer / AverageAmplitude; } }

    public bool NormalizeSamples { get { return normalizeSamples; } set { normalizeSamples = value; } }
    public bool SmoothSamples { get { return smoothSamples; } set { smoothSamples = value; } }
    public float SmoothingShift { get { return smoothingShift; } set { smoothingShift = value; } }
    public float SmoothingScale { get { return smoothingScale; } set { smoothingScale = value; } }
    public float MinNormalizedSampleValue { get { return minNormalizedSampleValue; } set { minNormalizedSampleValue = value; } }
    public float MaxNormalizedSampleValue { get { return maxNormalizedSampleValue; } set { maxNormalizedSampleValue = value; } }
    public float SamplingLoudnessMultiplier { get { return samplingLoudnessMultiplier; } set { samplingLoudnessMultiplier = value; } }

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            if (timeSinceLastBeat >= timeBetweenBeats)
            {
                OnBeat?.Invoke();
                timeSinceLastBeat = 0;
            }
            timeSinceLastBeat += Time.deltaTime;

            // Spectrum Data
            if (smoothSamples) { GetSmoothedSpectrumData(); }
            else { OnlyGetSpectrumData(); }

            if (normalizeSamples) { NormalizeSamplesData(); }

            MakeCombinedSpectrumData();

            // Make Frequency Bands
            MakeFrequencyBands();

            // Band Buffer
            CalcBandBuffer();

            // Create Audio Bands
            CreateAudioBands();

            // Calculate Amplitude
            GetAmplitude();
        }
    }

    public int ReadBPM(AudioClip clip)
    {
        int bpm = UniBpmAnalyzer.AnalyzeBpm(clip);
        if (bpm < estimatedBPMRange.x)
        {
            while (bpm < estimatedBPMRange.x) bpm *= 2;
        }
        if (bpm > estimatedBPMRange.y)
        {
            while (bpm > estimatedBPMRange.y) bpm /= 2;
        }

        this.bpm = bpm;
        timeBetweenBeats = 60f / bpm;
        timeSinceLastBeat = 0;

        return bpm;
    }

    private void NormalizeSamplesData()
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

    [ContextMenu("Reset Sample Normalization Values")]
    private void ResetSampleNormalizationValues()
    {
        cachedSmallestValueLeft = Mathf.Infinity;
        cachedLargestValueLeft = 0;
        cachedSmallestValueRight = Mathf.Infinity;
        cachedLargestValueRight = 0;
    }

    private void GetSmoothedSpectrumData()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);

        // initial pass
        for (int i = 0; i < numSamples; ++i)
        {
            // smooth
            cachedSmoothingValue = GetSmoothedValue(i, numSamples);
            leftAudioSamples[i] = leftAudioSamples[i] * cachedSmoothingValue * samplingLoudnessMultiplier;
            rightAudioSamples[i] = rightAudioSamples[i] * cachedSmoothingValue * samplingLoudnessMultiplier;
        }
    }

    private void OnlyGetSpectrumData()
    {
        audioSource.GetSpectrumData(leftAudioSamples, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(rightAudioSamples, 1, FFTWindow.Blackman);

        for (int i = 0; i < numSamples; ++i)
        {
            leftAudioSamples[i] *= samplingLoudnessMultiplier;
            rightAudioSamples[i] *= samplingLoudnessMultiplier;
        }
    }

    private void MakeCombinedSpectrumData()
    {
        for (int i = 0; i < numSamples; ++i) { combinedAudioSamples[i] = leftAudioSamples[i] + rightAudioSamples[i]; }
    }

    private float GetSmoothedValue(int i, int total)
    {
        return Mathf.Pow((((float)i / total) * smoothingScale) + smoothingShift, 2);
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

    public void SetClip(AudioClip clip)
    {
        CreateAudioProfile();

        ReadBPM(clip);
    }

    public float GetFrequencyBandValue(int band, bool useBuffer) { return useBuffer ? frequencyBandBuffer[band] : frequencyBands[band]; }
    public float GetAudioBandValue(int band, bool useBuffer) { return useBuffer ? audioBandsBuffer[band] : audioBands[band]; }
    public float GetAmplitudeValue(bool useBuffer) { return useBuffer ? amplitudeBuffer : amplitude; }
    public float GetAverageAmplitudeValue(bool useBuffer) { return useBuffer ? AverageAmplitudeBuffer : AverageAmplitude; }
    public float GetSampleValue(int sample, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.LEFT:
                return leftAudioSamples[sample];
            case AudioChannel.RIGHT:
                return rightAudioSamples[sample];
            case AudioChannel.STEREO:
                return combinedAudioSamples[sample];
            default:
                throw new UncaughtSwitchTypeException(typeof(AudioChannel), channel.ToString());
        }
    }
}
