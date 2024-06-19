using System.Collections;
using UnityEngine;

public abstract class GoalBasedTransition : Transition
{
    [SerializeField] protected float transitionSpeed;
    [SerializeField] protected MathHelper.AlterationMethod moveBy;
    protected float goal;

    protected override IEnumerator TransitionIn()
    {
        SetGoalIn();
        while (!HasPropertyReachedGoal())
        {
            MovePropertyTowardsGoal();

            yield return null;
        }
    }

    protected override IEnumerator TransitionOut()
    {
        SetGoalOut();
        while (!HasPropertyReachedGoal())
        {
            MovePropertyTowardsGoal();

            yield return null;
        }
    }

    protected abstract void MovePropertyTowardsGoal();

    protected abstract bool HasPropertyReachedGoal();
    protected abstract void SetGoalIn();
    protected abstract void SetGoalOut();
}
