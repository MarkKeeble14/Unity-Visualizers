public class RunCoverArtSelectionOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunCoverArtSelection());
    }
}
