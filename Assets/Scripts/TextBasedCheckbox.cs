using TMPro;
using UnityEngine;

public class TextBasedCheckbox : Checkbox
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string activeString;
    [SerializeField] private string inactiveString;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    protected override void IsActive()
    {
        text.text = activeString;
        text.color = activeColor;
    }

    protected override void IsInactive()
    {
        text.text = inactiveString;
        text.color = inactiveColor;
    }
}