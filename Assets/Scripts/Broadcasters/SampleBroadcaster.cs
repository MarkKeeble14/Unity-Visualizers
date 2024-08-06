using FFMpegCore.Enums;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;

public class SampleBroadcaster : SignalBroadcaster
{
    [Header("Attach To")]
    [SerializeField] private int sample;
    public int Sample { get { return sample; } set { sample = value; } }
    [SerializeField] private AudioChannel channel = AudioChannel.STEREO;
    public AudioChannel Channel { get { return channel; } set { channel = value; } }

    public override float GetBroadcastValue()
    {
        return AudioSamplingManager._Instance.GetSampleValue(sample, channel) * signalMultiplier;
    }
}