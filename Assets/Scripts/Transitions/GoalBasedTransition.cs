using System.Collections;
using UnityEngine;

public abstract class GoalBasedTransition : Transition
{
    [SerializeField] protected float transitionInSpeed;
    [SerializeField] protected float transitionOutSpeed;
    [SerializeField] protected MathHelper.AlterationMethod moveBy;
    protected float goal;

    protected override IEnumerator TransitionIn(float speed = 1)
    {
        SetGoalIn();
        while (!HasPropertyReachedGoal())
        {
            MovePropertyTowardsGoal(speed);

            yield return null;
        }
    }

    protected override IEnumerator TransitionOut(float speed = 1)
    {
        SetGoalOut();
        while (!HasPropertyReachedGoal())
        {
            MovePropertyTowardsGoal(speed);

            yield return null;
        }
    }

    protected abstract void MovePropertyTowardsGoal(float speed);

    protected abstract bool HasPropertyReachedGoal();
    protected abstract void SetGoalIn();
    protected abstract void SetGoalOut();
}
