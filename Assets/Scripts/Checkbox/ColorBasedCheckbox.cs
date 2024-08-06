using UnityEngine;
using UnityEngine.UI;

public class ColorBasedCheckbox : Checkbox
{
    [SerializeField] private Image image;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    protected override void IsActive()
    {
        image.color = activeColor;
    }

    protected override void IsInactive()
    {
        image.color = inactiveColor;
    }
}
