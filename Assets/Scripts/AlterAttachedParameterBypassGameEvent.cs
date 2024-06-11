using UnityEngine;

public class AlterAttachedParameterBypassGameEvent : GameEvent
{
    [SerializeField] private AttachParameter attachedParameter;
    [SerializeField] private bool value;

    public override void Activate()
    {
        attachedParameter.Bypass = value;
    }
}
