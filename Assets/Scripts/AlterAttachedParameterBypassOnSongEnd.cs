using UnityEngine;

public class AlterAttachedParameterBypassOnSongEnd : AddToOnSongEnd
{
    [SerializeField] private AttachParameter attachedParameter;
    [SerializeField] private bool value;

    public override void CallOnSongEnd()
    {
        attachedParameter.Bypass = value;
    }
}
