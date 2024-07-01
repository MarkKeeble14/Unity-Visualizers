using System;
using TMPro;
using UnityEngine;

public class DropdownElement : MonoBehaviour
{
    private Action onClick;

    [SerializeField] private TextMeshProUGUI indexText;
    private int index;

    public int GetIndex()
    {
        return index;
    }

    public void SetIndex(int value)
    {
        index = value;
        indexText.text = value.ToString();
    }

    public void AddOnClick(Action a)
    {
        onClick += a;
    }

    public void OnClick()
    {
        onClick?.Invoke();
    }
}
