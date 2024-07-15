using UnityEngine;

public abstract class ActOnKeyPress : KeyControl
{
    private void Update()
    {
        if (Disable) return;
        if (Input.GetKeyDown(key))
        {
            Act();
        }
    }

    protected abstract void Act();
}
