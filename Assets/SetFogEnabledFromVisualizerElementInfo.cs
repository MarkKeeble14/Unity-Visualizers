using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFogEnabledFromVisualizerElementInfo : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        RenderSettings.fog = info[VisualizerElementLabel.FOG].Enabled;
    }
}
