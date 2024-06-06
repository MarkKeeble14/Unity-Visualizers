using UnityEngine;

public class BandBroadcaster : AudioSignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private bool useBuffer;
    [SerializeField] private bool bypass;

    [Header("Attach To")]
    [SerializeField] private int band;
    public int Band { get { return band; } set { band = value; } }

    protected override void TryBroadcast()
    {
            BroadcastMessage("RecieveBroadcast", VisualizerManager._Instance.GetBandValue(band, useBuffer) * signalMultiplier);
    }
}
