using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageColorToMatchTrack : SetElementColorToMatchTrack
{
    private Image image;

    protected override void SetElementVariable()
    {
        image = GetComponent<Image>();
    }

    protected override void SetElementColorByGradient(float percent)
    {
        image.color = trackGradient.Evaluate(percent);
    }

    protected override void SetElementToDominantColor()
    {
        image.color = dominant;
    }

    protected override void SetElementToSecondaryColor()
    {
        image.color = secondary;
    }

    protected override void SetElementToTertiaryColor()
    {
        image.color = tertiary;
    }
}
