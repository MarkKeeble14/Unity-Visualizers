using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachLightIntensity : AttachParameter
{
    [SerializeField] private Light light;
    protected override void SetParameter(float value)
    {
        light.intensity = value;
    }
}
