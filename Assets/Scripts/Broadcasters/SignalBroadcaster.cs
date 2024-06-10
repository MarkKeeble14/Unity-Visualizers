using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;


[RequireComponent(typeof(SignalDirector))]
public abstract class SignalBroadcaster : MonoBehaviour
{
    [SerializeField, Range(0, 10000)] protected float signalMultiplier = 1;

    public void UpdateSettings(float signalMultiplier)
    {
        this.signalMultiplier = signalMultiplier;
    }

    public abstract float GetBroadcastValue();
}
