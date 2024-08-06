using UnityEngine;

public abstract class AttachParameterFollowup : MonoBehaviour
{
    [SerializeField] private AttachParameter attachment;

    private void Start()
    {
        attachment.AddOnSetParameter(Followup);
    }

    protected abstract void Followup(float v);
}
