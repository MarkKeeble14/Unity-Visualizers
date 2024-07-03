using UnityEngine;

public class ValueEqualToParameterLock : ParameterLock
{
    [SerializeField] private float equalTo;

    public override bool EvaluateCondition(float value)
    {
        return value == equalTo;
    }
}
