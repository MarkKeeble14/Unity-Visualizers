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
        SetFontInfo(VisualizerManager._Instance.GetFontName(index), VisualizerManager._Instance.GetFont(index));
    }

    public override void OpenListSelection()
    {
        StartCoroutine(VisualizerManager._Instance.SelectOneFont(
            (filePath, font) =>
            {
                VisualizerManager._Instance.UpdateFont(filePath, Index);
                SetFontInfo(VisualizerManager._Instance.GetFontName(Index), VisualizerManager._Instance.GetFont(Index));
            }));
    }

    private void SetFontInfo(string fontName, TMP_FontAsset newFont)
    {
        this.fontName.font = newFont;
        this.fontName.text = fontName;
        exampleText.font = newFont;
    }
}
