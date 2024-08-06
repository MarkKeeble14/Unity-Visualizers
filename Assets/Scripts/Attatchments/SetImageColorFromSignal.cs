using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImageColorFromSignal : AttachParameter
{
    [SerializeField] private Image image;
    [SerializeField] private float v1;
    [SerializeField] private Color c1;
    [SerializeField] private Color c2;

    protected override void SetParameter(float value)
    {
        if (value == v1)
        {
            image.color = c1;
        } else
        {
            image.color = c2;
        }
    }
}
