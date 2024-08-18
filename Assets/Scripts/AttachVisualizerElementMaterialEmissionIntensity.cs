using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachVisualizerElementMaterialEmissionIntensity : AttachParameter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private Material material;
    [SerializeField] private string setColorKey = "_EmissionColor";
    [SerializeField] private VisualizerElementLabel label;
    private Color color;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(label)) return;
        {
            color = VisualizerManager._Instance.GetColor(info[label].ColorType, info[label].ColorIndex);
        }
    }

    protected override void SetParameter(float value)
    {
        material.SetColor(setColorKey, color * value);
    }
}