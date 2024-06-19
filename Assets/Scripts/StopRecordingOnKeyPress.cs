public class StopRecordingOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        ScreenRecorder._Instance.StopRecording();
    }
}