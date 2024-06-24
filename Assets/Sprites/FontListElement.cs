using UnityEngine;
using TMPro;

public class FontListElement : ListSelectionElement
{
    [SerializeField] private TextMeshProUGUI exampleText;
    [SerializeField] private TextMeshProUGUI fontName;

    public void Set(int index, TMP_FontAsset font)
    {
        SetIndex(index);
        SetFontInfo(MakeFontNameFromAsset(font), font);
    }

    public void Set(int index, TMP_FontAsset font, string filePath)
    {
        SetIndex(index);
        SetFontInfo(StringHelper.GetFileName(filePath), font);
    }

    public override void OpenListSelection()
    {
        StartCoroutine(VisualizerManager._Instance.SelectOneFont(
            (filePath, font) =>
            {
                SetFontInfo(StringHelper.GetFileName(filePath), font);
                VisualizerManager._Instance.UpdateFont(Index, font);
            }));
    }

    private void SetFontInfo(string fontName, TMP_FontAsset newFont)
    {
        this.fontName.font = newFont;
        this.fontName.text = fontName;
        exampleText.font = newFont;
    }

    private string MakeFontNameFromAsset(TMP_FontAsset font)
    {
        return font.ToString().Split(" (")[0];
    }
}
