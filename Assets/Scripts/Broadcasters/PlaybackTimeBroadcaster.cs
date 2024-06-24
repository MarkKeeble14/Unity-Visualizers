public class PlaybackTimeBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.PlaybackTime;
    }
}
