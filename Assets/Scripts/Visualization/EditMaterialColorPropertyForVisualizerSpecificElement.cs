using System.Collections.Generic;
using UnityEngine;

public class EditMaterialColorPropertyForVisualizerSpecificElement : DatabaseSetter, IRecieveVisualizerElementsInfo, IRecieveVisualizerFloatValues
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private string colorProperty = "_Color";
    [SerializeField] private Material mat;
    private Color color;
    private float emissionIntensity;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(label.ToString(), SettingType.EMISSION_INTENSITY.ToString()), x =>
        {
            emissionIntensity = x;
        });

        mat.SetColor(colorProperty, color * emissionIntensity);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            color = VisualizerManager._Instance.GetColor(x.ColorType, x.ColorIndex);

            mat.SetColor(colorProperty, color * emissionIntensity);
        });
    }
}
