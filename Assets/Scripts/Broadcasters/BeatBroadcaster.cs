public class BeatBroadcaster : SignalBroadcaster
{
    private float beatValue;

    private void Start()
    {
        VisualizerManager._Instance.OnBeat += x => beatValue = x;
    }

    public override float GetBroadcastValue()
    {
        float returning = beatValue;
        beatValue = 0;
        return returning;
    }
}
