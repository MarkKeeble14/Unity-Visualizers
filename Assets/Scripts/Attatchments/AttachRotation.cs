using UnityEngine;

public class AttachRotation : AttachParameter
{
    [SerializeField] private Axis rotateOn;
    [SerializeField] private Transform rotateTarget;

    protected override void SetParameter(float value)
    {
        // Setting Value
        switch (rotateOn)
        {
            case Axis.X:
                rotateTarget.localEulerAngles = new Vector3(value, 0, 0);
                break;
            case Axis.Y:
                rotateTarget.localEulerAngles = new Vector3(0, value, 0);
                break;
            case Axis.Z:
                rotateTarget.localEulerAngles = new Vector3(0, 0, value);
                break;
        }
    }
}