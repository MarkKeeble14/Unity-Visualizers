using UnityEngine;
using UnityEngine.UI;

public class ButtonComputerScreenControls : ComputerScreenControl
{
    [SerializeField] private Button button;

    public override void Clicked()
    {
        button.onClick?.Invoke();
    }

    public override void WhileHovered()
    {
        //
    }

    public override void WhileNotHovered()
    {
        //
    }
}
