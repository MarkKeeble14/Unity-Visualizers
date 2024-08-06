using UnityEngine;
using TMPro;
using System;

public class FontListElement : ListSelectionElement
{
    [SerializeField] private TextMeshProUGUI exampleText;
    [SerializeField] private TextMeshProUGUI fontName;

    public void Set(int index)
    {
        SetIndex(index);
        SetFontInfo(VisualizerManager._Instance.GetFont(index));
    }

    public override void Open()
    {
        StartCoroutine(VisualizerManager._Instance.BrowseForFont(
            (filePath, font) =>
            {
                VisualizerManager._Instance.UpdateFont(filePath, Index);
                SetFontInfo(VisualizerManager._Instance.GetFont(Index));
            }));
    }

    private void SetFontInfo(TMP_FontAsset newFont)
    {
        fontName.font = newFont;
        fontName.text = VisualizerManager._Instance.GetFontName(Index);
        exampleText.font = newFont;
    }

    public override void Delete()
    {
        VisualizerManager._Instance.DeleteFont(Index);
    }

    protected override void Randomize()
    {
        //
    }
}
