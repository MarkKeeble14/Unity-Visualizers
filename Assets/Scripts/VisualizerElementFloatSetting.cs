using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizerElementFloatSetting : MonoBehaviour, IRecieveVisualizerFloatValues
{
    [SerializeField] private string label;
    [SerializeField] private string key;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        labelText.text = label;
    }

    public void RecieveVisualizerFloatValues(Dictionary<string, float> settings)
    {
        if (!settings.ContainsKey(key))
        {
            return;
        }

        inputField.text = settings[key].ToString();
    }

    public void UpdateSetting(string s)
    {
        float v;
        if (float.TryParse(s, out v))
        {
            UpdateSetting(v);
        }
    }

    private void UpdateSetting(float v)
    {
        VisualizerManager._Instance.UpdateFloatSetting(key, v);
    }
}
