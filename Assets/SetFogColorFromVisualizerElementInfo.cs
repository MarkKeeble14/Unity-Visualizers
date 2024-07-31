using System.Collections.Generic;
using UnityEngine;

public class SetFogColorFromVisualizerElementInfo : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        RenderSettings.fogColor = 
            VisualizerManager._Instance.GetColor(info[VisualizerElementLabel.FOG].ColorType, info[VisualizerElementLabel.FOG].ColorIndex);
    }
}