using TMPro;
using System;
using UnityEngine;

public class FontDropdownMenu : DropdownMenu, IRecieveTrackInfo
{
    public Action<TMP_FontAsset> OnFontSelected;

    public void RecieveTrackInfo(TrackInfo info)
    {
        Clear();

        foreach (TMP_FontAsset f in info.Fonts)
        {
            FontDropdownElement e = (FontDropdownElement)CreateElementObject();
            e.SetFont(f);
        }
    }

    protected override void SetElementActive(int index)
    {
        TMP_FontAsset f;
        if (index >= dropdownElements.Count)
        {
            f = VisualizerManager._Instance.GetDefaultFont();
        } else
        {
            f = ((FontDropdownElement)dropdownElements[index]).GetFont();
        }
        labelText.font = f;
        OnFontSelected?.Invoke(f);
    }
}
