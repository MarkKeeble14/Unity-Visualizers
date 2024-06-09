using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;

public class SampleBroadcaster : SignalBroadcaster
{
    [Header("Attach To")]
    [SerializeField] private int sample;
    public int Sample { get { return sample; } set { sample = value; } }

    protected override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.AudioSamples[sample] * signalMultiplier;
    }
}