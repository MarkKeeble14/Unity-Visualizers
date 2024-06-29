using UnityEngine;
using UnityEngine.UI;

public class ColorBasedCheckbox : Checkbox
{
    [SerializeField] private Image image;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    protected override void UpdateUI()
    {
        image.color = (Active ? activeColor : inactiveColor);
    }
}
