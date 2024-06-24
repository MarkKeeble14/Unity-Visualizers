using UnityEngine;

public class ValueLessThanParameterLock : ParameterLock
{
    [SerializeField] private float lessThan;

    public override bool EvaluateCondition(float value)
    {
        return value < lessThan;
    }
}
