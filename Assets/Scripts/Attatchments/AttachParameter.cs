using UnityEngine;
using UnityEngine.XR;

public abstract class AttachParameter : MonoBehaviour
{
    [SerializeField] private bool Bypass;

    [Header("Adjustment Settings")]
    [SerializeField] protected bool slowAdust;
    [SerializeField] private float defaultValue = 0;
    [SerializeField] private float adjustSpeed = 25;
    [SerializeField] private float multiplier = 1;
    protected float targetValue;
    protected float currentValue;
    

    public void SetSlowAdjust(bool newValue) { slowAdust = newValue; }

    protected abstract void SetParameter(float value);

    protected void RecieveBroadcast(float v)
    {
        if (Bypass) return;
        v *= multiplier;
        if (slowAdust)
        {
            // Calculate Adjustment
            targetValue = defaultValue + v;
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * adjustSpeed);

            SetParameter(currentValue);
        }
        else
        {
            SetParameter(defaultValue + v);
        }
    }
}
