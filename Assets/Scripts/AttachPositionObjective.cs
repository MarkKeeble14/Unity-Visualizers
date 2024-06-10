using UnityEngine;

public class AttachPositionObjective : AttachPosition
{
    protected override void SetParameter(float value)
    {
        switch (axis)
        {
            case Axis.X:
                transform.position = new Vector3(value, transform.position.y, transform.position.z);
                break;
            case Axis.Y:
                transform.position = new Vector3(transform.position.x, value, transform.position.z);
                break;
            case Axis.Z:
                transform.position = new Vector3(transform.position.x, transform.position.y, value);
                break;
        }
    }
}
