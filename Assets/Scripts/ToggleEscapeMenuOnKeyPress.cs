using UnityEngine;

public class ToggleEscapeMenuOnKeyPress : ActOnKeyPress
{
    [SerializeField] private EscapeMenuFunctions escapeMenu;

    protected override void Act()
    {
        escapeMenu.Toggle();
    }
}
