using UnityEngine;

public class AttachPositionObjective : AttachPosition
{
    [SerializeField] private Transform transformToAlter;
    protected override void SetParameter(float value)
    {
        switch (axis)
        {
            case Axis.X:
                transformToAlter.position = new Vector3(value, transformToAlter.position.y, transformToAlter.position.z);
                break;
            case Axis.Y:
                transformToAlter.position = new Vector3(transformToAlter.position.x, value, transformToAlter.position.z);
                break;
            case Axis.Z:
                transformToAlter.position = new Vector3(transformToAlter.position.x, transformToAlter.position.y, value);
                break;
        }
    }
}
