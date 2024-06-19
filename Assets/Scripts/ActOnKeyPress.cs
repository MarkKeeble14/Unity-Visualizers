using UnityEngine;

public abstract class ActOnKeyPress : MonoBehaviour
{
    [SerializeField] private KeyCode key;

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Act();
        }
    }

    protected abstract void Act();
}
