using System.Collections.Generic;
using UnityEngine;

public class CallPromptExit : MonoBehaviour
{
    public void PromptExit()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Eject Disk?", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("Confirm", () => Application.Quit())
        }));
    }
}