using System.Collections;
using UnityEngine;

public class ScaleOnHover : TweenOnHover
{
    [SerializeField] private float hoveredScale;
    [SerializeField] private float unhoveredScale;
    private float currentGoal;

    public override void Hovered()
    {
        currentGoal = hoveredScale;
    }

    public override void NotHovered()
    {
        currentGoal = unhoveredScale;
    }

    protected override void Tween()
    {
        transform.localScale = Vector3.one * MathHelper.GetNextValue(transform.localScale.x, currentGoal, tweenSpeed, tweenMethod, true);
    }
}
