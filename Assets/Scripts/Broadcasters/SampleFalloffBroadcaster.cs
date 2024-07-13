using UnityEngine;

public class SampleFalloffBroadcaster : FalloffBroadcaster
{
    [Header("Band Settings")]
    [SerializeField] private int sample;
    public int Sample { get { return sample; } set { sample = value; } }

    protected override float GetValue()
    {
        return VisualizerManager._Instance.AudioSamples[sample];
    }
}
