using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerBoolSetting : VisualizerSetting, IRecieveVisualizerBoolValues
{
    [SerializeField] private Image i;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;
    private bool active;

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

        active = values[key];
        i.color = (active ? activeColor : inactiveColor);
    }

    public void Toggle()
    {
        VisualizerManager._Instance.UpdateSetting(key, !active);
    }
}