using UnityEngine;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetColor(Color c)
    {
        text.color = c;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
