using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class AttachParameter : MonoBehaviour
{
    [Header("Flow Settings")]
    [SerializeField] private List<int> listenOnFrequencies = new List<int>() { 0 };
    [SerializeField] private bool bypass;
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

    protected void RecieveBroadcast(SignalBroadcastObject message)
    {
        // if this attachment isn't listening to that specific frequency, don't act
        if (!listenOnFrequencies.Contains(message.FrequencyId)) return;

        // if set to bypass, don't act
        if (bypass) return;

        // if a min value requirement has been set and the currently broadcasted message does not transcend it, don't act
        if (useMinValueRequirement && message.Value < minValueRequirement) return;

        lastMessageValue = message.Value;
        message.Value *= multiplier;
        if (slowAdust)
        {
            // Calculate Adjustment
            targetValue = defaultValue + message.Value;
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * adjustSpeed);

            SetParameter(currentValue);
        }
        else
        {
            SetParameter(defaultValue + message.Value);
        }
    }
}
