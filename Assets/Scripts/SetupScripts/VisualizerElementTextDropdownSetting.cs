using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementTextDropdownSetting : VisualizerSetting, IRecieveVisualizerIntValues
{
    [SerializeField] private TextDropdownMenu dropdown;

    public void SetOptions(string[] options)
    {
        dropdown.PopulateDropdown(options);
    }

    public void RecieveVisualizerIntValues(Dictionary<string, int> settings)
    {
        if (!settings.ContainsKey(key))
        {
            return;
        }

        TrySetDefaultValue(settings[key]);

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

    public override void SetToDefaultValue()
    {
        int i;
        if (int.TryParse(defaultValue, out i))
        {
            VisualizerManager._Instance.UpdateSetting(key, i);
        }
    }
}