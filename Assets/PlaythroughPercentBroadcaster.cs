using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaythroughPercentBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.PlaythroughPercent;
    }
}
