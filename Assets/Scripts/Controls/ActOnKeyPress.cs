using UnityEngine;

public abstract class ActOnKeyPress : KeyControl
{
    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Act();
        }
    }

    protected abstract void Act();
}
