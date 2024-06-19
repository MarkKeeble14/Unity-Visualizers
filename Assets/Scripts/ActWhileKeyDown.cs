using UnityEngine;

public abstract class ActWhileKeyDown : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    protected abstract void Act();

    private void Update()
    {
        if (Input.GetKey(key))
        {
            Act();
        }
    }
}

