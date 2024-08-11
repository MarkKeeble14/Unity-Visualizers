using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetBloomTintFromVisualizerElementInfo : BloomSetter, IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, VisualizerElementLabel.BLOOM, x => bloom.tint.Override(VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex)));
    }
}