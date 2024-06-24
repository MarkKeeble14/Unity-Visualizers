public class ResetPlaybackOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        VisualizerManager._Instance.ResetPlayback();
    }
}
