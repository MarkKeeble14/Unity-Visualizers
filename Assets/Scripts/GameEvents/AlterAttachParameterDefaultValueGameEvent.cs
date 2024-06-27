using System.Collections.Generic;
using UnityEngine;

public class AlterAttachParameterDefaultValueGameEvent : AlterFloatGameEvent
{
    [SerializeField] private AttachParameter attachment;

    protected override float GetValue()
    {
        return attachment.DefaultValue;
    }

    protected override void SetValue(float newValue)
    {
        attachment.DefaultValue = newValue;
    }
}
