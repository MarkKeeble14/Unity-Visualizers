using System.Collections.Generic;
using UnityEngine;

public class EditMaterialColorPropertyForVisualizerSpecificElement : DatabaseSetter, IRecieveVisualizerElementsInfo, IRecieveVisualizerFloatValues
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private string colorProperty = "_Color";
    [SerializeField] private Material mat;
    private float emissionIntensity;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(label.ToString(), SettingType.EMISSION_INTENSITY.ToString()), x =>
        {
            emissionIntensity = x;
        });

        mat.SetColor(colorProperty, mat.GetColor(colorProperty) * emissionIntensity);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            Color c = VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex);
            mat.SetColor(colorProperty, c * emissionIntensity);
        });
    }
}
