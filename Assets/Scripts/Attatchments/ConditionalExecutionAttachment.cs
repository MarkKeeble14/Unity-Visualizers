using UnityEngine;

public abstract class  ConditionalExecutionAttachment : AttachParameter
{
    [SerializeField] private ParameterLock conditionLock;
    protected bool isLocked = true;

    protected override void TrySetParameter(float value)
    {
        isLocked = !conditionLock.EvaluateCondition(value);
        if (isLocked)
        {
            SetParameterCalledWhileLocked(value);
        }
        else
        {
            SetParameterCalledWhileNotLocked(value);
        }
    }

    protected virtual void SetParameterCalledWhileLocked(float value) { SetParameter(value); }

    protected virtual void SetParameterCalledWhileNotLocked(float value) { SetParameter(value); }
}
