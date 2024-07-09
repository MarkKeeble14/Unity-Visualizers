using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementDropdownSetting : MonoBehaviour, IRecieveVisualizerIntValues
{
    [SerializeField] private string label;
    [SerializeField] private string key;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private DropdownMenu dropdown;

    private void Start()
    {
        labelText.text = label;

        dropdown.OnSelectElement += UpdateSetting;
    }

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
            UpdateSetting(v);
        }
    }

    private void UpdateSetting(int v)
    {
        VisualizerManager._Instance.UpdateIntSetting(key, v);
    }
}