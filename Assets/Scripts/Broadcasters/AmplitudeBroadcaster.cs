using UnityEngine;

public class AmplitudeBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private bool useBuffer = true;
    [SerializeField] private bool useAverage;

    public override float GetBroadcastValue()
    {
        return useAverage 
            ? AudioSamplingManager._Instance.GetAverageAmplitudeValue(useBuffer) 
            : AudioSamplingManager._Instance.GetAmplitudeValue(useBuffer);
    }
}
