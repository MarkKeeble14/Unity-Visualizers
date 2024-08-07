using UnityEngine;

public class BouncingFloatBroadcaster : SignalBroadcaster
{
    [SerializeField] private AnimationDirection startDirection;
    [SerializeField] private float speed;
    public float Speed { get { return speed; } set {  speed = value; } }
    [SerializeField] private Vector2 minMaxValue;
    private float currentValue;
    private float targetValue;

    private void Start()
    {
        DecideTarget();
    }

    private void DecideTarget()
    {
        switch (startDirection)
        {
            case AnimationDirection.FORWARD:
                targetValue = minMaxValue.y;
                break;
            case AnimationDirection.BACKWARD:
                targetValue = minMaxValue.x;
                break;
            case AnimationDirection.RANDOM:
                if (RandomHelper.RandomBool())
                    targetValue = minMaxValue.y;
                else
                    targetValue = minMaxValue.x;
                break;
        }
    }

    protected void Update()
    {
        currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);
        if (currentValue >= minMaxValue.y)
        {
            targetValue = minMaxValue.x;
        } else if (currentValue <= minMaxValue.x)
        {
            targetValue = minMaxValue.y;
        }
    }

    public override float GetBroadcastValue()
    {
        return currentValue * signalMultiplier;
    }

    public void ResetValues()
    {
        currentValue = 0;
        DecideTarget();
    }
}
