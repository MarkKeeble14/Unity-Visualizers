using UnityEngine;

public abstract class AutomateInputContinuous : MonoBehaviour
{
    [SerializeField] protected RecievesInput automating;

    private void Update()
    {
        Automate();
    }

    protected abstract void Automate();
}

