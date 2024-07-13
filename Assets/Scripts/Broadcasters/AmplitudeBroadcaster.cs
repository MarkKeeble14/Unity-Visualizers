using UnityEngine;

public class AmplitudeBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private bool useBuffer = true;
    [SerializeField] private bool useAverage;

    public override float GetBroadcastValue()
    {
        return useAverage 
            ? VisualizerManager._Instance.GetAverageAmplitudeValue(useBuffer) 
            : VisualizerManager._Instance.GetAmplitudeValue(useBuffer);
    }
}
