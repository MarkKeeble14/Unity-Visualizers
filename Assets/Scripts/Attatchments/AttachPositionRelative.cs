using UnityEngine;

public class AttachPositionRelative : AttachPosition
{
    [SerializeField] private Transform transformToAlter;
    protected override void SetParameter(float value)
    {
        value *= Time.deltaTime;
        switch (axis)
        {
            case Axis.X:
                transformToAlter.position += new Vector3(value, 0, 0);
                break;
            case Axis.Y:
                transformToAlter.position += new Vector3(0, value, 0);
                break;
            case Axis.Z:
                transformToAlter.position += new Vector3(0, 0, value);
                break;
        }
    }
}
