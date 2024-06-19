public class PausePlaybackOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        if (VisualizerManager._Instance.IsPlaybackPaused)
        {
            VisualizerManager._Instance.ResumePlayback();
        } else
        {
            VisualizerManager._Instance.PausePlayback();
        }
    }
}
