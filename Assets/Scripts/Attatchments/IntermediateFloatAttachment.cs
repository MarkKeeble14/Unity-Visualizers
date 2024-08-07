using UnityEngine;

public abstract class IntermediateFloatAttachment : ConditionalExecutionAttachment
{
    [Header("Intermediate Float")]
    [SerializeField] private Vector2 minMaxValue;
    [SerializeField] private float increaseSpeed;
    [SerializeField] private MathHelper.AlterationMethod increaseAlterationBy;
    [SerializeField] private float decreaseSpeed;
    [SerializeField] private MathHelper.AlterationMethod decreaseAlterationBy;
    private float intermediateValue;

    private void Update()
    {
        if (isLocked)
        {
            intermediateValue = MathHelper.GetNextValue(intermediateValue, minMaxValue.x, decreaseSpeed, decreaseAlterationBy, true);
        }
        else
        {
            intermediateValue = MathHelper.GetNextValue(intermediateValue, minMaxValue.y, increaseSpeed, increaseAlterationBy, true);
        }
    }

    protected override void SetParameterCalledWhileLocked(float value) { SetParameter(intermediateValue); }

    protected override void SetParameterCalledWhileNotLocked(float value) { SetParameter(intermediateValue); }

    public void SetMinMaxValue(float x, float y)
    {
        minMaxValue = new Vector2(x, y);
    }

    public void SetMinMaxValue(Vector2 v)
    {
        minMaxValue = v;
    }
}
