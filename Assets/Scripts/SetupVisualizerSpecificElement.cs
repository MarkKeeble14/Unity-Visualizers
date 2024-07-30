using System.Collections.Generic;
using UnityEngine;

public class SetupVisualizerSpecificElement : SetupVisualizerElement, IRecieveVisualizerSpecificElementsInfo
{
    [SerializeField] private string key;

    public string Key => key;

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(key))
        {
            VisualizerManager._Instance.RegisterVisualizerSpecificElement(key, new VisualizerElementsSettings(VisualizerColorType.COLOR, 0, 0, true));
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
        VisualizerManager._Instance.UpdateVisualizerSpecificElementsSettings(key, newSettings);
    }
}
