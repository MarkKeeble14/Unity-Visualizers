using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AttachTMPFontSize : AttachParameter
{
    [SerializeField] private TextMeshProUGUI text;

    protected override void SetParameter(float value)
    {
        text.fontSize = value;
    }
}
