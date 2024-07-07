using System.Collections.Generic;
using UnityEngine;

public class SetupVisualizerSpecificElement : SetupVisualizerElement, IRecieveVisualizerSpecificElementsInfo
{
    [SerializeField] private string key;

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(key))
        {
            Set();
            return;
        }

        colorType = info[key].ColorType;
        colorIndex = info[key].ColorIndex;
        fontIndex = info[key].FontIndex;
        active = info[key].Enabled;

        Set();
    }

    protected override VisualizerElementsSettings GetElementSettings()
    {
        return VisualizerManager._Instance.GetVisualizerSpecificElementSettings(key);
    }

    protected override void UpdateSettings(VisualizerElementsSettings newSettings)
    {
        VisualizerManager._Instance.SetVisualizerSpecificElementsSettings(key, newSettings);
    }
}
