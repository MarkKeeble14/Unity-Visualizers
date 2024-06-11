public class SecondsPlayedBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return VisualizerManager._Instance.SecondsPlayed;
    }
}
