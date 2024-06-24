public class RunColorsSelectionOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunColorsSelection());
    }
}
