using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeBroadcastValue : SignalBroadcaster
{
    [SerializeField] private SignalBroadcaster normalizingValueOf;

    [SerializeField] private float minimumInput;
    public float MinimumInput { get { return minimumInput; } set { minimumInput = value; } }

    [SerializeField] private float maximumInput;
    public float MaximumInput { get { return maximumInput; } set { maximumInput = value; } }

    [SerializeField] private float minimumOutput;
    public float MinimumOutput { get { return minimumOutput; } set { minimumOutput = value; } }

    [SerializeField] private float maximumOutput;
    public float MaximumOutput { get { return maximumOutput; } set { maximumOutput = value; } }

    public override float GetBroadcastValue()
    {
        return MathHelper.Normalize(normalizingValueOf.GetBroadcastValue(), minimumInput, maximumInput, minimumOutput, maximumOutput);
    }
}
