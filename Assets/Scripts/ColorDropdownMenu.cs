using UnityEngine;
using System;
using UnityEngine.UI;

public class ColorDropdownMenu : DropdownMenu, IRecieveTrackInfo
{
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
        Color c = ((ColorDropdownElement)dropdownElements[index]).GetColor();
        labelTextBackground.color = c;
        OnColorSelected?.Invoke(c);
    }
}
