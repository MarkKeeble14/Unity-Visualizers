using System.Collections.Generic;
using UnityEngine;

public class SetupVisualizerSpecificElement : SetupVisualizerElement, IRecieveVisualizerSpecificElementsInfo
{
    [SerializeField] private string visualizerSpecificElementKey;

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(visualizerSpecificElementKey))
        {
            Set();
            return;
        }

        colorType = info[visualizerSpecificElementKey].ColorType;
        colorIndex = info[visualizerSpecificElementKey].ColorIndex;
        fontIndex = info[visualizerSpecificElementKey].FontIndex;
        active = info[visualizerSpecificElementKey].Enabled;

        Set();
    }

    protected override VisualizerElementsSettings GetElementSettings()
    {
        return VisualizerManager._Instance.GetVisualizerSpecificElementSettings(visualizerSpecificElementKey);
    }

    protected override void UpdateSettings(VisualizerElementsSettings newSettings)
    {
        VisualizerManager._Instance.SetVisualizerSpecificElementsSettings(visualizerSpecificElementKey, newSettings);
    }
}
