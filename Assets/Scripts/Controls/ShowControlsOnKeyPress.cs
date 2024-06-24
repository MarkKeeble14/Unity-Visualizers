using UnityEngine;

public class ShowControlsOnKeyPress : ActOnKeyPress
{
    [SerializeField] private ControlPanel controlPanel;

    protected override void Act()
    {
        controlPanel.SetCanvasGroupAlpha(1);
    }
}
