using System.Collections.Generic;
using UnityEngine;

public class SetLightAttachmentsFromVisualizerFloats: SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private AttachParameter lightAttachment;

    [Header("Keys")]
    [SerializeField] private SettingType defaultValue;
    [SerializeField] private SettingType multiplier;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(defaultValue, values, x => lightAttachment.DefaultValue = x);
        TrySetValueFromDatabase(multiplier, values, x => lightAttachment.Multiplier = x);
    }
}
