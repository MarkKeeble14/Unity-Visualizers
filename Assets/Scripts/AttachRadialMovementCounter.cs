using UnityEngine;

public class AttachRadialMovementCounter : AttachParameter
{
    [SerializeField] private RadialMovement radialMovement;

    protected override void SetParameter(float value)
    {
        radialMovement.SetRadiusPosition(value);
    }
}
