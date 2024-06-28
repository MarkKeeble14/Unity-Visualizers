public class SaveVisualizerPresetOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        VisualizerManager._Instance.SavePreset();
    }
}
