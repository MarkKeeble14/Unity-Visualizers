using System.Collections.Generic;
using UnityEngine;

public class SetAttachParameterDefaultValueFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    [SerializeField] private AttachParameter attachment;
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private SettingType settingLabel;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(label.ToString(), settingLabel.ToString()), x => attachment.DefaultValue = x);
    }
}
