using TMPro;
using UnityEngine;

public abstract class VisualizerElementInputSetting : VisualizerSetting
{
    [SerializeField] protected TMP_InputField inputField;

    public void OnInput(string str)
    {
        UpdateSetting(str);
    }

    protected abstract void UpdateSetting(string str);

    public override void SetToDefaultValue()
    {
        OnInput(defaultValue);
    }
}