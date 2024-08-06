using UnityEngine;

public class SnareHitBroadcaster : SignalBroadcaster
{
    [Header("Settings")]
    [SerializeField] private MathHelper.AlterationMethod method;
    [SerializeField] private float falloffSpeed;
    [SerializeField] private int baseSpikeValue;
    [SerializeField] private float energySpikeValueMultiplier = 1;
    private float currentValue;

    private void Start()
    {
        AudioSamplingManager._Instance.OnSnareHit +=
            x =>
            {
                currentValue = baseSpikeValue + (x * energySpikeValueMultiplier);
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
