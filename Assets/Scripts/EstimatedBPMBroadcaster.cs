using System.Collections;
using System.Collections.Generic;

public class EstimatedBPMBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.GetEstimatedBPM();
    }
}
