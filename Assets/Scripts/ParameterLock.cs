using UnityEngine;

public abstract class ParameterLock : MonoBehaviour
{
    public abstract bool EvaluateCondition(float value);
}
