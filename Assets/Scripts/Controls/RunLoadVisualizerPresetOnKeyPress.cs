public class RunLoadVisualizerPresetOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunLoadPresetSelection());
    }
}
