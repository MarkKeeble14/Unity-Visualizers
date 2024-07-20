using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SetTextColorToMatchTrack : SetBaseVisualizerElementColorToTrackColor
{
    private TextMeshProUGUI text;

    protected override void SetElementVariable()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    protected override void SetElementToColor(Color c)
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        text.color = c;
    }
}
