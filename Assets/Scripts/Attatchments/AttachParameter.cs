using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class AttachParameter : MonoBehaviour
{
    [Header("Flow Settings")]
    [SerializeField] private bool bypass;
    public bool Bypass { get { return bypass; } set {  bypass = value; } }
    [SerializeField] private ParameterLock shouldExecuteLock;

    [Header("Adjustment Settings")]
    [SerializeField] protected bool slowAdust;
    public bool SlowAdjust { get { return slowAdust; } set {  slowAdust = value; } }

    [SerializeField] private float adjustSpeed = 25;
    public float AdjustSpeed { get { return adjustSpeed; } set { adjustSpeed = value; } }
    [SerializeField] private float defaultValue = 0;
    public float DefaultValue { get { return defaultValue; } set {  defaultValue = value; } }

    [SerializeField] private float multiplier = 1;
    public float Multiplier { get { return multiplier; } set { multiplier = value; } }

    protected float targetValue;
    protected float currentValue;

    private Action<float> onSetParameter;

    protected virtual void TrySetParameter(float value) { SetParameter(value); }

    protected abstract void SetParameter(float value);

    public void RecieveBroadcast(float value)
    {
        // if set to bypass, don't act
        if (bypass) return;

        // if a min value requirement has been set and the currently broadcasted message does not transcend it, don't act
        if (shouldExecuteLock && !shouldExecuteLock.EvaluateCondition(value)) return;

        value *= multiplier;
        // lastMessageValue = value;
        if (slowAdust)
        {
            // Calculate Adjustment
            targetValue = defaultValue + value;
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * adjustSpeed);
        }
        else
        {
            currentValue = defaultValue + value;
            
        }
        TrySetParameter(currentValue);
        onSetParameter?.Invoke(currentValue);
    }

    public void AddOnSetParameter(Action<float> action)
    {
        onSetParameter += action;
    }

    public void RemoveOnSetParameter(Action<float> action)
    {
        onSetParameter -= action;
    }
}
