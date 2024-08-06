using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementFloatSetting : VisualizerSetting, IRecieveVisualizerFloatValues
{
    [SerializeField] private TMP_InputField inputField;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        SetInputFieldText(values[key]);
    }

    public void UpdateSetting(string s)
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
