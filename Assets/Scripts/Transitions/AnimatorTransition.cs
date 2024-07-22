using System.Collections;
using UnityEngine;

public abstract class AnimatorTransition : Transition
{
    [SerializeField] protected Animator animator;

    private bool hasStarted = false;
    private bool hasCompleted = false;
    private TransitionDirection currentTransitionDirection;

    public void SetCompleted()
    {
        hasCompleted = true;
        hasStarted = false;

        TransitionEnd();
        switch (currentTransitionDirection)
        {
            case TransitionDirection.IN:
                TransitionInEnd();
                break;
            case TransitionDirection.OUT:
                TransitionOutEnd();
                break;
        }
    }

    protected abstract void TransitionInEnd();
    protected abstract void TransitionOutEnd();
    protected abstract void TransitionEnd();

    protected abstract void BeforeTransitionIn();
    protected abstract void BeforeTransitionOut();

    protected abstract void WhileTransitioningIn();
    protected abstract void WhileTransitioningOut();

    protected override IEnumerator TransitionIn(float speed)
    {
        BeforeTransitionIn();

        hasStarted = true;
        hasCompleted = false;
        animator.speed = speed;
        animator.SetTrigger("In");
        currentTransitionDirection = TransitionDirection.IN;
        yield return new WaitUntil(() => hasCompleted);
    }

    protected override IEnumerator TransitionOut(float speed)
    {
        BeforeTransitionOut();

        hasStarted = true;
        hasCompleted = false;
        animator.speed = speed;
        animator.SetTrigger("Out");
        currentTransitionDirection = TransitionDirection.OUT;
        yield return new WaitUntil(() => hasCompleted);
    }

    private void Update()
    {
        if (!hasCompleted && hasStarted)
        {
            switch (currentTransitionDirection)
            {
                case TransitionDirection.IN:
                    WhileTransitioningIn();
                    break;
                case TransitionDirection.OUT:
                    WhileTransitioningOut();
                    break;
            }
        }
    }
}
