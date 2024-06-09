using UnityEngine;

public class CirclingFloatBroadcaster : SignalBroadcaster
{
    [SerializeField] private AnimationDirection startDirection;
    [SerializeField] private float speed;
    private float directionMultiplier;
    private float currentValue;

    private void Start()
    {
        switch (startDirection)
        {
            case AnimationDirection.FORWARD:
                directionMultiplier = 1;
                break;
            case AnimationDirection.BACKWARD:
                directionMultiplier = -1;
                break;
            case AnimationDirection.RANDOM:
                if (RandomHelper.RandomBool())
                    directionMultiplier = 1;
                else
                    directionMultiplier = -1;
                break;
        }
    }

    protected override void Update()
    {
        currentValue += Time.deltaTime * speed * directionMultiplier;

        base.Update();
    }

    protected override float GetBroadcastValue()
    {
        return (currentValue % 360f) * signalMultiplier;
    }
}