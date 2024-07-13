using UnityEngine;

public class NewHighestAmplitudePeakBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private MathHelper.AlterationMethod method;
    [SerializeField] private float falloffSpeed;
    [SerializeField] private int baseSpikeValue;
    [SerializeField] private float ampSpikeValueMultiplier = 1;
    private float currentValue;

    private void Start()
    {
        VisualizerManager._Instance.OnNewAmplitudePeak +=
            x =>
            {
                currentValue = baseSpikeValue + (x * ampSpikeValueMultiplier);
            };
    }

    private void Update()
    {
        currentValue = MathHelper.GetNextValue(currentValue, 0, falloffSpeed, method, true);
    }

    public override float GetBroadcastValue()
    {
        return currentValue;
    }
}
