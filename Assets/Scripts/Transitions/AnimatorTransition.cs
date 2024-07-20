using System.Collections;
using UnityEngine;

public abstract class AnimatorTransition : Transition
{
    [SerializeField] protected Animator animator;

    private bool hasCompleted = false;

    public void SetCompleted()
    {
        hasCompleted = true;
    }

    protected abstract void BeforeTransitionIn();
    protected abstract void BeforeTransitionOut();

    protected override IEnumerator TransitionIn()
    {
        BeforeTransitionIn();

        hasCompleted = false;
        animator.SetTrigger("In");
        yield return new WaitUntil(() => hasCompleted);
    }

    protected override IEnumerator TransitionOut()
    {
        BeforeTransitionOut();

        hasCompleted = false;
        animator.SetTrigger("Out");
        yield return new WaitUntil(() => hasCompleted);
    }
}
