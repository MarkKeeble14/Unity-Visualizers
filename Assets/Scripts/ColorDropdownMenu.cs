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

        SelectElement(selectedIndex);
    }

    protected override void SetElementActive(int index)
    {
        if (dropdownElements.Count == 0)
        {
            return;
        }

        if (index >= dropdownElements.Count)
        {
            index = dropdownElements.Count - 1;
        }
        Color c = ((ColorDropdownElement)dropdownElements[index]).GetColor();
        colorDisplay.color = c;
        OnColorSelected?.Invoke(c);
    }
}
