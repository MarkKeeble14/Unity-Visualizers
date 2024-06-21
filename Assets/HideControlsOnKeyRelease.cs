using UnityEngine;

public class HideControlsOnKeyRelease : ActOnKeyRelease
{
    [SerializeField] private ControlPanel controlPanel;

    protected override void Act()
    {
        controlPanel.SetCanvasGroupAlpha(0);
    }
}
