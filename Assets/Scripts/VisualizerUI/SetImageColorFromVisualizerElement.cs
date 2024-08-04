using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageColorFromVisualizerElement : SetVisualizerElementColor
{
    private Image image;

    protected override void SetElementVariable()
    {
        image = GetComponent<Image>();
    }

    protected override void SetElementToColor(Color c)
    {
        if (image == null) image = GetComponent<Image>();
        image.color = c;
    }
}
