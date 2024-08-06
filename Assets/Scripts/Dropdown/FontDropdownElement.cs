using TMPro;
using UnityEngine;

public class FontDropdownElement : DropdownElement
{
    private TMP_FontAsset font;
    [SerializeField] private TextMeshProUGUI fontDisplayText;

    public TMP_FontAsset GetFont()
    {
        return font;
    }

    public void SetFont(TMP_FontAsset font)
    {
        this.font = font;
        fontDisplayText.font = font;
    }
}
