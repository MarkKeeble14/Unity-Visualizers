using UnityEngine;

public class AttachPositionRelative : AttachPosition
{
    protected override void SetParameter(float value)
    {
        value *= Time.deltaTime;
        switch (axis)
        {
            case Axis.X:
                transform.position += new Vector3(value, 0, 0);
                break;
            case Axis.Y:
                transform.position += new Vector3(0, value, 0);
                break;
            case Axis.Z:
                transform.position += new Vector3(0, 0, value);
                break;
        }
    }
}
