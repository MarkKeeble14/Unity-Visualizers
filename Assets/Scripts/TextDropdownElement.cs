using TMPro;
using UnityEngine;

public class TextDropdownElement : DropdownElement
{
    [SerializeField] private TextMeshProUGUI textDisplay;

    private string text;

    public string GetText() => text;

    public void SetText(string s)
    {
        text = s;
        textDisplay.text = s;
    }
}