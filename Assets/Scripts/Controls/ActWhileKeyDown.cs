using UnityEngine;

public abstract class ActWhileKeyDown : KeyControl
{
    protected abstract void Act();

    private void Update()
    {
        if (Disable) return;
        if (Input.GetKey(key))
        {
            Act();
        }
    }
}

