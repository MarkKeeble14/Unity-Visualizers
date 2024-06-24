using UnityEngine;

public class TransitionOnKeyPress : ActOnKeyPress
{
    [SerializeField] private string transitionType;
    [SerializeField] private TransitionDirection direction;

    protected override void Act()
    {
        TransitionManager._Instance.Transition(transitionType, direction);
    }
}
