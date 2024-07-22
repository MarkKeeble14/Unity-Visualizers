using UnityEngine;
using UnityEngine.UI;

public class BlackFadeTransition : GoalBasedTransition
{
    [SerializeField] private Image image;
    [SerializeField] private float equalityTolerence = .1f;
    private Color c;

    protected override bool HasPropertyReachedGoal()
    {
        return Mathf.Abs(image.color.a - goal) < equalityTolerence;
    }

    protected override void MovePropertyTowardsGoal(float speed = 1)
    {
        c = image.color;
        c.a = MathHelper.GetNextValue(c.a, goal, (goal == 1 ? transitionInSpeed : transitionOutSpeed) * speed, moveBy, true);
        image.color = c;
    }

    protected override void SetGoalIn()
    {
        goal = 1;
    }

    protected override void SetGoalOut()
    {
        goal = 0;
    }
}