using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[RequireComponent(typeof(RectTransform))]
public class AttachHeight : AttachParameter
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected override void SetParameter(float value)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
    }
}
