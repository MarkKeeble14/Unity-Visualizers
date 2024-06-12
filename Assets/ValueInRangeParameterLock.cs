using UnityEngine;

public class ValueInRangeParameterLock : ParameterLock
{
    [SerializeField] private Vector2 range;

    public override bool EvaluateCondition(float value)
    {
        return value > range.x && value < range.y;
    }
}
