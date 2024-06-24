public class RunFontSelectionOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunFontsSelection());
    }
}
