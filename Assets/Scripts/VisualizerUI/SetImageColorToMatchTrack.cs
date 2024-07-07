using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageColorToMatchTrack : SetBaseVisualizerElementColorToTrackColor
{
    private Image image;

    protected override void SetElementVariable()
    {
        image = GetComponent<Image>();
    }

    protected override void SetElementToColor(Color c)
    {
        image.color = c;
    }
}
