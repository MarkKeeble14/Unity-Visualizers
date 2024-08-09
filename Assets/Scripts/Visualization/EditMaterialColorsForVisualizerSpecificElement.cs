using System.Collections.Generic;
using UnityEngine;


public class EditMaterialColorsForVisualizerSpecificElement : DatabaseSetter, IRecieveVisualizerElementsInfo, IRecieveVisualizerFloatValues
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Material mat;
    private float emissionIntensity;

    private void SetColors(Color c, float emissionIntensity)
    {
        mat.SetColor("_BaseColor", c);
        mat.SetColor("_EmissionColor", c * emissionIntensity);
    }

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(label.ToString(), SettingType.EMISSION_INTENSITY.ToString()), x =>
        {
            emissionIntensity = x;
        });

        SetColors(mat.GetColor("_BaseColor"), emissionIntensity);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            Color c = VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex);
            SetColors(c, emissionIntensity);
        });
    }
}
