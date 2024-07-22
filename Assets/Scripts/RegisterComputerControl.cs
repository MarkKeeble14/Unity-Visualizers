using UnityEngine;

public class RegisterComputerControl : MonoBehaviour
{
    [SerializeField] private ComputerScreenControl[] controls;

    private void Start()
    {
        if (ComputerCursor._Instance == null) return;
        ComputerCursor._Instance.AddInteractables(controls);
    }

    private void OnDestroy()
    {
        if (ComputerCursor._Instance == null) return;
        ComputerCursor._Instance.RemoveInteractables(controls);
    }
}
