using System;
using TMPro;
using UnityEngine;

public class ActionSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Action onClick;

    public void Set(ActionSelection actionSelection)
    {
        text.text = actionSelection.Text;
        onClick = actionSelection.Action;
    }

    public void CallOnClick()
    {
        onClick?.Invoke();
    }
}
