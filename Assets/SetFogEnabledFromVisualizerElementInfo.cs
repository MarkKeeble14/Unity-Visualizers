using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFogEnabledFromVisualizerElementInfo : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, VisualizerElementLabel.FOG, x => RenderSettings.fog = x.Enabled);
    }
}
