using System.Collections.Generic;
using UnityEngine;

public class SetVisualizerSpecificElementLight : MonoBehaviour, IRecieveVisualizerSpecificElementsInfo
{
    [SerializeField] private string key;
    [SerializeField] private Light light;

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(key)) return;
        VisualizerElementsSettings settings = info[key];
        light.enabled = settings.Enabled;
        light.color = VisualizerManager._Instance.GetColor(settings.ColorType, settings.ColorIndex);
    }
}