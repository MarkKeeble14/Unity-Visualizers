using UnityEngine;
using UnityEngine.XR;

public class BandBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private bool useBuffer;
    [SerializeField] private bool bypass;

    [Header("Attach To")]
    [SerializeField] private int band;
    public int Band { get { return band; } set { band = value; } }

    protected override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.GetBandValue(band, useBuffer) * signalMultiplier;
    }
}
