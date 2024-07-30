using System.Collections.Generic;
using UnityEngine;

public class SetupBasicVisualizerElement : SetupVisualizerElement, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;

    public VisualizerElementLabel Label => label;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        colorType = info[label].ColorType;
        colorIndex = info[label].ColorIndex;
        fontIndex = info[label].FontIndex;
        active = info[label].Enabled;

        Set();
    }

    protected override VisualizerElementsSettings GetElementSettings()
    {
        return VisualizerManager._Instance.GetBaseVisualizerElementSettings(label);
    }

    protected override void UpdateSettings(VisualizerElementsSettings newSettings)
    {   
        VisualizerManager._Instance.UpdateBaseVisualizerElementSettings(label, newSettings);
    }
}
