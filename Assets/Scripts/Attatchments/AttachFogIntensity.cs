using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachFogIntensity : AttachParameter
{
    protected override void SetParameter(float value)
    {
        RenderSettings.fogDensity = value;
    }
}
