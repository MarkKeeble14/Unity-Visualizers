public class TimeOfDayBroadcaster : SignalBroadcaster
{
    public override float GetBroadcastValue()
    {
        return DayNightCycleManager._Instance.PercentThroughDay;
    }
}
