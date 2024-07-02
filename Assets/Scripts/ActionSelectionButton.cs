using System;
using TMPro;
using UnityEngine;

public class ActionSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public Action OnClick;

    public void Set(ActionSelection action)
    {
        text.text = action.Text;
        OnClick += action.Action;
    }

    public void CallOnClick()
    {
        OnClick?.Invoke();
    }
}
