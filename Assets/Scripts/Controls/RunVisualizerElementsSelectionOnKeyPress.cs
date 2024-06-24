public class RunVisualizerElementsSelectionOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunVisualizerElementsSelection());
    }
}
