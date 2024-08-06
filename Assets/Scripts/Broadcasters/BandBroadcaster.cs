using UnityEngine;

public class BandBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private bool useBuffer = true;
    [SerializeField] private BandType bandType = BandType.FREQUENCY;

    [Header("Attach To")]
    [SerializeField] private int band;
    public int Band { get { return band; } set { band = value; } }

    public override float GetBroadcastValue()
    {
        switch (bandType)
        {
            case BandType.FREQUENCY:
                return AudioSamplingManager._Instance.GetFrequencyBandValue(band, useBuffer) * signalMultiplier;
            case BandType.AUDIO:
                return AudioSamplingManager._Instance.GetAudioBandValue(band, useBuffer) * signalMultiplier;
            default:
                throw new UncaughtSwitchTypeException(typeof(BandType), bandType.ToString());
        }
    }
}
