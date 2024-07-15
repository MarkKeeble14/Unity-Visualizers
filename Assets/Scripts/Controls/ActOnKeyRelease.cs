using UnityEngine;

public abstract class ActOnKeyRelease : KeyControl
{
    private void Update()
    {
        if (Disable) return;
        if (Input.GetKeyUp(key))
        {
            Act();
        }
    }

    protected abstract void Act();
}
