using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementFloatSetting : VisualizerElementInputSetting, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        TrySetDefaultValue(values[key]);

        SetInputFieldText(values[key]);
    }

    protected override void UpdateSetting(string s)
    {
        float v;
        if (float.TryParse(s, out v))
        {
            VisualizerManager._Instance.UpdateSetting(key, v);
        }
    }

    public void SetInputFieldText(float x)
    {
        inputField.text = x.ToString();
    }

    protected override void Initialize()
    {
        // 
    }
}
