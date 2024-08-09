using System.Collections.Generic;
using UnityEngine;

public class SetVisualizerElementLightColorAndEnabled : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Light light;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            light.enabled = x.Enabled;
            light.color = VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex);
        });
    }
}