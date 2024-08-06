using UnityEngine;

public class SampleFalloffBroadcaster : FalloffBroadcaster
{
    [Header("Sample Settings")]
    [SerializeField] private int sample;
    public int Sample { get { return sample; } set { sample = value; } }

    [SerializeField] private AudioChannel channel = AudioChannel.STEREO;
    public AudioChannel Channel { get { return channel; } set { channel = value; } }

    protected override float GetValue()
    {
        return AudioSamplingManager._Instance.GetSampleValue(sample, channel);
    }
}
