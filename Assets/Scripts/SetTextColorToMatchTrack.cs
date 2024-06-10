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

    protected override void SetElementToColor(Color c)
    {
        text.color = c;
    }
}
