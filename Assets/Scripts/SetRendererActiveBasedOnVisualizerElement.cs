using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRendererActiveBasedOnVisualizerElement : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Renderer renderer;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x => renderer.enabled = x.Enabled);
    }
}
