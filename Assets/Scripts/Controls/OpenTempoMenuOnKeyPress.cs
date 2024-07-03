public class OpenTempoMenuOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        VisualizerManager._Instance.OpenTempoMenu();
    }
}
