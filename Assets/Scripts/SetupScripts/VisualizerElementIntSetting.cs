using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementIntSetting : VisualizerElementInputSetting, IRecieveVisualizerIntValues
{
    public void RecieveVisualizerIntValues(Dictionary<string, int> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        TrySetDefaultValue(values[key]);

        inputField.text = values[key].ToString();
    }

    protected override void UpdateSetting(string s)
    {
        int v;
        if (int.TryParse(s, out v))
        {
            VisualizerManager._Instance.UpdateSetting(key, v);
        }
    }

    protected override void Initialize()
    {
        // 
    }
}
