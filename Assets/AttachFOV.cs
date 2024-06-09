using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachFOV : AttachParameter
{
    [SerializeField] private Camera camera;
    protected override void SetParameter(float value)
    {
        camera.fieldOfView = value;
    }
}
