using UnityEngine;
using UnityEngine.Events;

public class CallFunctionOnKeyPress : ActOnKeyPress
{
    [SerializeField] private UnityEvent func;

    protected override void Act()
    {
        func?.Invoke();
    }
}
