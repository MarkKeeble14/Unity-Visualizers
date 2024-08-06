using UnityEngine;

public class DragUIComputerScreenControls : ComputerScreenControl
{
    [SerializeField] private DraggableUI draggableUI;

    protected override void LoadEvents()
    {
        clicked += () => draggableUI.BeginDrag(ComputerCursor._Instance.CursorRect.anchoredPosition);
        released += () => draggableUI.EndDrag();
        held += () => draggableUI.Drag(ComputerCursor._Instance.CursorRect.anchoredPosition);
    }
}
