using UnityEngine;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Set(string text)
    {
        this.text.text = text;
    }
}
