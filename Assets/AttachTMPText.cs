using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttachTMPText : AttachParameter
{
    [Header("Attach TMP Text Settings")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool shouldRound;
    [SerializeField] private int roundTo;

    protected override void SetParameter(float value)
    {
        text.text = (shouldRound ? System.Math.Round(value, roundTo).ToString() : value.ToString());
    }
}
