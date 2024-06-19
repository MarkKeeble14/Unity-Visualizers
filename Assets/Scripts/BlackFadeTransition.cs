using UnityEngine;
using UnityEngine.UI;

public class BlackFadeTransition : GoalBasedTransition
{
    [SerializeField] private Image image;
    [SerializeField] private float initializeGoalTo = 1;
    private Color c;

    private void Start()
    {
        goal = initializeGoalTo;
    }

    protected override bool HasPropertyReachedGoal()
    {
        return Mathf.Approximately(image.color.a, goal);
    }

    protected override void MovePropertyTowardsGoal()
    {
        c = image.color;
        c.a = MathHelper.GetNextValue(c.a, goal, transitionSpeed, moveBy, true);
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