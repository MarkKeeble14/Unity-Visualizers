using UnityEngine;
using UnityEngine.Events;

public class CallFunctionWhileKeyDown : ActWhileKeyDown
{
    [SerializeField] private UnityEvent func;

    protected override void Act()
    {
        func?.Invoke();
    }
}
