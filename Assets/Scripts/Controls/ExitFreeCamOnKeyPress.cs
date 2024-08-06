public class ExitFreeCamOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        FreeCameraController._Instance.Deactivate();
    }
}
