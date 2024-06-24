using UnityEngine;

public abstract class ActOnKeyRelease : KeyControl
{
    private void Update()
    {
        if (Input.GetKeyUp(key))
        {
            Act();
        }
    }

    protected abstract void Act();
}
