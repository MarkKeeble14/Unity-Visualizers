using System.Collections;
using System.Collections.Generic;

public class BPMValueBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.BPM;
    }
}
