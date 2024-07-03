using UnityEngine;
using System;
using UnityEngine.UI;

public class ColorDropdownMenu : DropdownMenu, IRecieveTrackInfo
{
    [SerializeField] private Image colorDisplay;
    public Action<Color> OnColorSelected;

    public void RecieveTrackInfo(TrackInfo info)
    {
        Clear();

        foreach (Color c in info.Colors)
        {
            ColorDropdownElement e = (ColorDropdownElement)CreateElementObject();
            e.SetColor(c);
        }
    }

    protected override void SetElementActive(int index)
    {
        if (index >= dropdownElements.Count) { return; }
        Color c = ((ColorDropdownElement)dropdownElements[index]).GetColor();
        colorDisplay.color = c;
        OnColorSelected?.Invoke(c);
    }
}
