using UnityEngine;

public class AttachRotation : AttachParameter
{
    [SerializeField] private Axis rotateOn;

    protected override void SetParameter(float value)
    {
        // Setting Value
        switch (rotateOn)
        {
            case Axis.X:
                transform.localEulerAngles = new Vector3(value, 0, 0);
                break;
            case Axis.Y:
                transform.localEulerAngles = new Vector3(0, value, 0);
                break;
            case Axis.Z:
                transform.localEulerAngles = new Vector3(0, 0, value);
                break;
        }
    }
}