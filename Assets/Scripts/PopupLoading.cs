using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupLoading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public string Text => text.text;


    public void Set(string text)
    {
        this.text.text = text;
    }
}
