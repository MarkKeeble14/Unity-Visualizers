using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AttachScale : AttachParameter
{
    [SerializeField] private Axis scaleOn;

    protected override void SetParameter(float value)
    {
        switch (scaleOn)
        {
            case Axis.X:
                transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
                break;
            case Axis.Y:
                transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z);
                break;
            case Axis.Z:
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value);
                break;
        }
    }
}
