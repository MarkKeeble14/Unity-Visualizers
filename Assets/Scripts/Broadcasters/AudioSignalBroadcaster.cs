using UnityEngine;

public abstract class AudioSignalBroadcaster : MonoBehaviour
{
    [SerializeField, Range(0, 10000)] protected float signalMultiplier = 1;

    public void UpdateSettings(float signalMultiplier)
    {
        this.signalMultiplier = signalMultiplier;
    }

    protected void Broadcast()
    {
        try
        {
            TryBroadcast();
        } catch
        {

        }
    }

    private void Update()
    {
        Broadcast();
    }

    protected abstract void TryBroadcast();
}
