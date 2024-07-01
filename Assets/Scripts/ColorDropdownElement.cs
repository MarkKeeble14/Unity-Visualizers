using UnityEngine;
using UnityEngine.UI;

public class ColorDropdownElement : DropdownElement
{
    [SerializeField] private Image colorDisplay;

    private Color color;

    public Color GetColor() => color;

    public void SetColor(Color c)
    {
        color = c;
        colorDisplay.color = c;
    }
}
