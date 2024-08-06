using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform tipContainer;
    public RectTransform TipContainer => tipContainer;

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
