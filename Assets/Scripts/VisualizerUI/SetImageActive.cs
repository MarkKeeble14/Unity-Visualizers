using UnityEngine;
using UnityEngine.UI;

public class SetImageActive : SetElementColorToMatchTrack
{
    [SerializeField] private Image image;

    protected override void SetElementToColor(Color c)
    {
        if (c.a == 0)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
        }
    }
}
