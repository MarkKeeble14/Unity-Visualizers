using UnityEngine;
using UnityEngine.Events;

public class CallFunctionOnHover : OnHoverHandler
{
    [SerializeField] private int funcCallsLimit;
    [SerializeField] private UnityEvent func;
    private int funcCallsThisHover;

    public override void Hovered()
    {
        if (funcCallsLimit >= 1)
        {
            if (funcCallsThisHover < funcCallsLimit)
            {
                CallFunc();
            }
        }
        else
        {
            CallFunc();
        }
    }

    public override void NotHovered()
    {
        funcCallsThisHover = 0;
    }

    private void CallFunc()
    {
        func?.Invoke();
        funcCallsThisHover++;
    }
}
