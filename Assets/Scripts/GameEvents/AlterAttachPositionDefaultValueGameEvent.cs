using System.Collections.Generic;
using UnityEngine;

public class AlterAttachPositionDefaultValueGameEvent : AlterFloatGameEvent
{
    [SerializeField] private AttachPositionRelative attachment;

    protected override float GetValue()
    {
        return attachment.DefaultValue;
    }

    protected override void SetValue(float newValue)
    {
        attachment.DefaultValue = newValue;
    }
}
