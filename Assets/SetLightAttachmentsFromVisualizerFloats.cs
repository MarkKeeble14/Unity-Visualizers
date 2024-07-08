using System.Collections.Generic;
using UnityEngine;

public class SetLightAttachmentsFromVisualizerFloats: MonoBehaviour, IRecieveVisualizerFloatValues
{
    [SerializeField] private AttachParameter lightAttachment;

    [Header("Keys")]
    [SerializeField] private string defaultValueKey;
    [SerializeField] private string multiplierKey;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        if (!values.ContainsKey(multiplierKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(multiplierKey, lightAttachment.Multiplier);
        }
        else
        {
            lightAttachment.Multiplier = values[multiplierKey];
        }

        if (!values.ContainsKey(defaultValueKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(defaultValueKey, lightAttachment.DefaultValue);
        }
        else
        {
            lightAttachment.DefaultValue = values[defaultValueKey];
        }
    }
}
