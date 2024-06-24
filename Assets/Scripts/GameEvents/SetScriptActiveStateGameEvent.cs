using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScriptActiveStateGameEvent : GameEvent
{
    [SerializeField] private MonoBehaviour monoBehaviour;
    [SerializeField] private bool value;

    public override void Activate()
    {
        monoBehaviour.enabled = value;
    }
}
