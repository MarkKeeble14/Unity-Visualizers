using UnityEngine;

public class AttachLightRange : AttachParameter
{
    [SerializeField] private Light light;
    protected override void SetParameter(float value)
    {
        light.range = value;
    }
}
