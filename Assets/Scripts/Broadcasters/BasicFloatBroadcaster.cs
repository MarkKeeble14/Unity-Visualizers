using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFloatBroadcaster : SignalBroadcaster
{
    [SerializeField, Range(-1, 1)] private float directionMultiplier;
    [SerializeField] private float speed = 1;
    [SerializeField] private float startingValue;
    private float currentValue;

    protected void Awake()
    {
        currentValue = startingValue;
    }

    protected void Update()
    {
        currentValue += Time.deltaTime * speed * directionMultiplier;
    }

    public override float GetBroadcastValue()
    {
        return currentValue;
    }
}
