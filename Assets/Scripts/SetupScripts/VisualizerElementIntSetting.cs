using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementIntSetting : VisualizerSetting, IRecieveVisualizerIntValues
{
    [SerializeField] private TMP_InputField inputField;

    public void RecieveVisualizerIntValues(Dictionary<string, int> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        inputField.text = values[key].ToString();
    }

    public void UpdateSetting(string s)
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
