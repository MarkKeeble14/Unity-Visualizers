using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class AttachParameter : MonoBehaviour
{
    [Header("Flow Settings")]
    [SerializeField] private bool bypass;
    public bool Bypass { get { return bypass; } set {  bypass = value; } }
    [SerializeField] private bool useMinValueRequirement;
    [SerializeField] private float minValueRequirement;

    [Header("Adjustment Settings")]
    [SerializeField] protected bool slowAdust;
    [SerializeField] private float defaultValue = 0;
    [SerializeField] private float adjustSpeed = 25;
    [SerializeField] private float multiplier = 1;
    protected float targetValue;
    protected float currentValue;
    private float lastMessageValue;

    public void SetSlowAdjust(bool newValue) { slowAdust = newValue; }

    protected abstract void SetParameter(float value);

    public void RecieveBroadcast(float value)
    {
        // if set to bypass, don't act
        if (bypass) return;

        // if a min value requirement has been set and the currently broadcasted message does not transcend it, don't act
        if (useMinValueRequirement && value < minValueRequirement) return;

        lastMessageValue = value;
        value *= multiplier;
        if (slowAdust)
        {
            // Calculate Adjustment
            targetValue = defaultValue + value;
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * adjustSpeed);

            SetParameter(currentValue);
        }
        else
        {
            SetParameter(defaultValue + value);
        }
    }
}
