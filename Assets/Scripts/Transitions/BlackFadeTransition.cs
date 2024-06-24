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

    protected override void MovePropertyTowardsGoal()
    {
        c = image.color;
        c.a = MathHelper.GetNextValue(c.a, goal, (goal == 255 ? transitionInSpeed : transitionOutSpeed), moveBy, true);
        image.color = c;
    }

    protected override void SetGoalIn()
    {
        goal = 255;
    }

    protected override void SetGoalOut()
    {
        goal = 0;
    }
}