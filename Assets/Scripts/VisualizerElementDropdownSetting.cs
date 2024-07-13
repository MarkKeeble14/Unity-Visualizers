using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementDropdownSetting : VisualizerSetting, IRecieveVisualizerIntValues
{
    [SerializeField] private DropdownMenu dropdown;

    public void RecieveVisualizerIntValues(Dictionary<string, int> settings)
    {
        if (!settings.ContainsKey(key))
        {
            return;
        }

        dropdown.ActivateElementAtIndex(settings[key]);
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
        dropdown.OnSelectElement += v => VisualizerManager._Instance.UpdateSetting(key, v);
    }
}