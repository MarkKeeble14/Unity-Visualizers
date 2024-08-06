using System.Collections.Generic;
using UnityEngine;

public class SetFogColorFromVisualizerElementInfo : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, VisualizerElementLabel.FOG, x =>
        {
            RenderSettings.fogColor = 
                VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex);
        });
    }
}