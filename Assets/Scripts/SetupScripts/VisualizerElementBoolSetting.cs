using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerElementBoolSetting : VisualizerSetting, IRecieveVisualizerBoolValues
{
    [SerializeField] private Checkbox checkbox;

    public void RecieveVisualizerBoolValues(Dictionary<string, bool> values)
    {
        if (!values.ContainsKey(key))
        {
            return;
        }

        TrySetDefaultValue(values[key]);

        checkbox.Active = values[key];
    }

    public void Set(bool b)
    {
        checkbox.Active = b;
        VisualizerManager._Instance.UpdateSetting(key, b);
    }

    public override void SetToDefaultValue()
    {
        bool b;
        if (bool.TryParse(defaultValue, out b))
        {
            checkbox.Active = b;
        }
    }

    public void Toggle()
    {
        VisualizerManager._Instance.UpdateSetting(key, !checkbox.Active);
    }

    protected override void Initialize()
    {
        // 
    }
}