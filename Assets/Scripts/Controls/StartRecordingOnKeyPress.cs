public class StartRecordingOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        ScreenRecorder._Instance.StartRecording();
    }
}
