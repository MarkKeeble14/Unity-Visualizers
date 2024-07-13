using UnityEngine;

public class BandFalloffBroadcaster : FalloffBroadcaster
{
    [Header("Settings")]
    [SerializeField] private int band;
    [SerializeField] private BandType bandType = BandType.FREQUENCY;
    public int Band { get { return band; } set { band = value; } }

    protected override float GetValue()
    {
        switch (bandType)
        {
            case BandType.FREQUENCY:
                return VisualizerManager._Instance.GetFrequencyBandValue(band, false) * signalMultiplier;
            case BandType.AUDIO:
                return VisualizerManager._Instance.GetAudioBandValue(band, false) * signalMultiplier;
            default:
                throw new UncaughtSwitchTypeException(typeof(BandType)); // TODO: Custom Exception
        }
    }
}
