using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCanvasGroupOnKeyPress : ActOnKeyPress
{
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void Act()
    {
        if (canvasGroup.alpha == 0)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        } else
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
