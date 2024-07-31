using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerElementBoolSetting : VisualizerSetting, IRecieveVisualizerBoolValues
{
    [SerializeField] private Checkbox checkbox;

    protected override void Initialize()
    {
        // 
    }

    public void RecieveVisualizerBoolValues(Dictionary<string, bool> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        checkbox.Active = values[key];
    }

    public void Set(bool b)
    {
        checkbox.Active = b;
        VisualizerManager._Instance.UpdateSetting(key, b);
    }

    public void Toggle()
    {
        VisualizerManager._Instance.UpdateSetting(key, !checkbox.Active);
    }
}