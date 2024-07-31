using System.Collections.Generic;
using UnityEngine;

public class SetVisualizerElementLight : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Light light;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(label)) return;
        VisualizerElementsSettings settings = info[label];
        light.enabled = settings.Enabled;
        light.color = VisualizerManager._Instance.GetColor(settings.ColorType, settings.ColorIndex);
    }
}