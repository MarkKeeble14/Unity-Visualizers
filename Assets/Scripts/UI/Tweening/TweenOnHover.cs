using UnityEngine;

public abstract class TweenOnHover : OnHoverHandler
{
    [SerializeField] protected float tweenSpeed;
    [SerializeField] protected MathHelper.AlterationMethod tweenMethod;

    private void Awake()
    {
        onUpdate += Tween;
    }

    protected abstract void Tween();
}
