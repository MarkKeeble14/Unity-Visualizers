using UnityEngine;

public class ValueGreaterThanParameterLock : ParameterLock
{
    [SerializeField] private float greaterThan;

    public override bool EvaluateCondition(float value)
    {
        return value > greaterThan;
    }
}
