public class OpenRecordingUIOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        ScreenRecorder._Instance.OpenRecordingUI();
    }
}

