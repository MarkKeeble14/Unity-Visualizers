using System;
using UnityEngine;

public class ComputerTransition : AnimatorTransition
{
    public void DisableAnimator()
    {
        animator.enabled = false;
    }

    protected override void BeforeTransitionIn()
    {
        // 
        animator.enabled = true;
    }

    protected override void BeforeTransitionOut()
    {
        // 
        animator.enabled = true;
    }
}
