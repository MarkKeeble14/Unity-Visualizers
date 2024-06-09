using UnityEditor.PackageManager.UI;
using UnityEngine;

public abstract class SignalBroadcaster : MonoBehaviour
{
    [SerializeField] protected int frequencyId;

    [SerializeField, Range(0, 10000)] protected float signalMultiplier = 1;

    public void UpdateSettings(float signalMultiplier)
    {
        this.signalMultiplier = signalMultiplier;
    }

    protected void Broadcast()
    {
        try
        {
            BroadcastMessage("RecieveBroadcast", new SignalBroadcastObject(frequencyId, GetBroadcastValue()));
        } catch
        {
            // 
        }
    }

    protected virtual void Update()
    {
        Broadcast();
    }

    protected abstract float GetBroadcastValue();
}
