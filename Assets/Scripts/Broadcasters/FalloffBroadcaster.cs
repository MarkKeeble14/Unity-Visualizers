using UnityEngine;

public abstract class FalloffBroadcaster : SignalBroadcaster
{
    [Header("Falloff Settings")]
    [SerializeField] private ParameterLock conditionalLock;
    [SerializeField] private float baseValue;
    [SerializeField] private float frequencyValueMultiplier;
    [SerializeField] private float cooldown;
    [SerializeField] private float valueFalloffSpeed;
    [SerializeField] private MathHelper.AlterationMethod method;
    private float cooldownTimer;
    private float currentValue;

    protected abstract float GetValue();

    private void Update()
    {
        currentValue = MathHelper.GetNextValue(currentValue, 0, valueFalloffSpeed, method, true);

        if (cooldownTimer > 0) { cooldownTimer -= Time.deltaTime; }
    }

    public override float GetBroadcastValue()
    {
        float v = GetValue();

        if (cooldownTimer > 0)
        {
            //
        }
        else if (
            (conditionalLock && conditionalLock.EvaluateCondition(v)) // if value passes condition
            || !conditionalLock) // or there is no condition
        {
            // set value and cooldown
            currentValue = baseValue + (v * frequencyValueMultiplier);
            cooldownTimer = cooldown;
        }
        return currentValue;
    }
}
