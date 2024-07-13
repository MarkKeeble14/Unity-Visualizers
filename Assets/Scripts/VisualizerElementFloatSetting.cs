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

        inputField.text = values[key].ToString();
    }

    public void UpdateSetting(string s)
    {
        float v;
        if (float.TryParse(s, out v))
        {
            VisualizerManager._Instance.UpdateSetting(key, v);
        }
    }

    protected override void Initialize()
    {
        // 
    }
}
