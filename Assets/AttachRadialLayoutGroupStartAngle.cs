using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachRadialLayoutGroupStartAngle : AttachParameter
{
    [SerializeField] private RadialLayoutGroup radialLayoutGroup;

    protected override void SetParameter(float value)
    {
        radialLayoutGroup.StartAngle = value;
    }
}
