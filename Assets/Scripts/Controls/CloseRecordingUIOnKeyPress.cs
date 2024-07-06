
public class CloseRecordingUIOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        ScreenRecorder._Instance.CloseRecordingUI();
    }
}

