using UnityEngine;

public class StopAllCoroutinesGameEvent : GameEvent
{
    [SerializeField] private MonoBehaviour monoBehaviour;

    public override void Activate()
    {
        monoBehaviour.StopAllCoroutines();
    }
}