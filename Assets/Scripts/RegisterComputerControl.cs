using UnityEngine;

public class RegisterComputerControl : MonoBehaviour
{
    [SerializeField] private ComputerScreenControl[] controls;
    private void Start()
    {
        ComputerCursor._Instance.AddInteractables(controls);
    }
}
