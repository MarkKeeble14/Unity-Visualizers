using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SetTextColorToMatchTrack : SetElementColorToMatchTrack
{
    private TextMeshProUGUI text;

    protected override void SetElementVariable()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    protected override void SetElementColorByGradient(float percent)
    {
        text.color = trackGradient.Evaluate(percent);
    }

    protected override void SetElementToDominantColor()
    {
        text.color = dominant;
    }

    protected override void SetElementToSecondaryColor()
    {
        text.color = secondary;
    }

    protected override void SetElementToTertiaryColor()
    {
        text.color = tertiary;
    }
}
