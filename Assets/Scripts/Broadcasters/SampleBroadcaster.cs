using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR;

public class SampleBroadcaster : AudioSignalBroadcaster
{
    [Header("Attach To")]
    [SerializeField] private int sample;
    public int Sample { get { return sample; } set { sample = value; } }

    protected override void TryBroadcast()
    {
        BroadcastMessage("RecieveBroadcast", VisualizerManager._Instance.AudioSamples[sample] * signalMultiplier);
    }
}