using UnityEngine;
using UnityEngine.UI;

public class ButtonComputerScreenControl : ComputerScreenControl
{
    [SerializeField] private Button button;

    protected override void LoadEvents()
    {
        clicked += () =>
        {
            if (!button.interactable) return;
            button.onClick?.Invoke();
        };
    }
}
