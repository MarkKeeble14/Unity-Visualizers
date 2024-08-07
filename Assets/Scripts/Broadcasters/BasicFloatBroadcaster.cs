using UnityEngine;

public class BasicFloatBroadcaster : SignalBroadcaster
{
    [SerializeField] private float value;

    public override float GetBroadcastValue()
    {
        return value * signalMultiplier;
    }
}
