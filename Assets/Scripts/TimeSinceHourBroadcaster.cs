using UnityEngine;

public class TimeSinceHourBroadcaster : SignalBroadcaster
{
    [SerializeField, Range(0, 24)] private float hour = 12;

    public override float GetBroadcastValue()
    {
        return DayNightCycleManager._Instance.GetTimeSinceHour(hour) / hour;
    }
}
