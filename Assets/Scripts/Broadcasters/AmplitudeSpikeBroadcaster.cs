using UnityEngine;

public class AmplitudeSpikeBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private MathHelper.AlterationMethod method;
    [SerializeField] private float falloffSpeed;
    private float ampSpikeTo;

    private void Start()
    {
        VisualizerManager._Instance.OnAmplitudeSpike += x => ampSpikeTo = x;
    }

    private void Update()
    {
        ampSpikeTo = MathHelper.GetNextValue(ampSpikeTo, 0, falloffSpeed, method, true);
    }

    public override float GetBroadcastValue()
    {
        return ampSpikeTo;
    }
}
