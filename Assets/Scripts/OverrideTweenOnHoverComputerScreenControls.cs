using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverrideTweenOnHoverComputerScreenControls : ComputerScreenControl
{
    [SerializeField] private TweenOnHover tweenOnHover;

    public override void Clicked()
    {
        //
    }

    public override void WhileHovered()
    {
        tweenOnHover.OverrideControl = true;
        tweenOnHover.Hovered();
    }

    public override void WhileNotHovered()
    {
        tweenOnHover.OverrideControl = false;
    }
}
