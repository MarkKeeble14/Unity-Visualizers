using System.Linq;
using UnityEngine;


[RequireComponent(typeof(SignalDirector))]
public abstract class SignalBroadcaster : MonoBehaviour
{
    [SerializeField, Range(0, 10000)] protected float signalMultiplier = 1;
    public float SignalMultiplier { get { return signalMultiplier; } set {  signalMultiplier = value; } }

    public void UpdateSettings(float signalMultiplier)
    {
        this.signalMultiplier = signalMultiplier;
    }

    public abstract float GetBroadcastValue();
}
