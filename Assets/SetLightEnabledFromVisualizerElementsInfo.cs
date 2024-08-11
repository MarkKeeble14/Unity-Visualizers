using System.Collections.Generic;
using UnityEngine;

public class SetLightEnabledFromVisualizerElementsInfo : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private Light light;
    [SerializeField] private VisualizerElementLabel key;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, key, x => light.enabled = x.Enabled);
    }
}
