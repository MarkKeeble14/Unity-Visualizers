using System.Collections;
using UnityEngine;

public abstract class AttachParameterLock : AttachParameter
{
    [Header("Lock Settings")]
    [SerializeField] protected int unlockedValue;
    [SerializeField] private bool instantChange;
    [SerializeField] private float changeRate;
    private bool locked = true;

    protected abstract void InstantUnlock();
    protected abstract void UpdateValue();
    protected abstract bool HasReachedFinalValue();
    
    private IEnumerator UnlockSlowly()
    {
        while (!HasReachedFinalValue())
        {
            UpdateValue();

            yield return null;
        }
    }

    protected float GetNextValue(float currentParameterValue)
    {
        return Mathf.Lerp(currentParameterValue, unlockedValue, Time.deltaTime * changeRate);
    }

    protected override void SetParameter(float value)
    {
        if (!locked) return;
        Unlock();
    }

    private void Unlock()
    {
        locked = false;

        if (instantChange)
            InstantUnlock();
        else
            StartCoroutine(UnlockSlowly());
    }

    [ContextMenu("ForceUnlock")]
    private void ForceUnlock()
    {
        Unlock();
    }
}
