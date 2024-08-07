using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallShowDesktopFunctions : MonoBehaviour
{
    [SerializeField] private CallLoadFileForDesktopBackground loadDesktopBackground;
    [SerializeField] private CallLoadTrackForDesktop loadDesktopTrack;

    public void ShowDesktopFunctions()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Load Menu", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("Desktop Background", () => loadDesktopBackground.LoadFile()),
            new ActionSelection("Track", () => loadDesktopTrack.LoadFile())
        }));
    }
}
