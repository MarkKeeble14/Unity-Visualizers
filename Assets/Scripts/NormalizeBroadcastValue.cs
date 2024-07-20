using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeBroadcastValue : SignalBroadcaster
{
    [SerializeField] private SignalBroadcaster normalizingValueOf;
    [SerializeField] private float minimumInput;
    [SerializeField] private float maximumInput;
    [SerializeField] private float minimumOutput;
    [SerializeField] private float maximumOutput;

    public override float GetBroadcastValue()
    {
        return MathHelper.Normalize(normalizingValueOf.GetBroadcastValue(), minimumInput, maximumInput, minimumOutput, maximumOutput);
    }
}
