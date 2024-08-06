using TMPro;
using UnityEngine;

public class ControlPanelSingleControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI actionText;

    protected override void MakeElement(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 1) throw new MalformedKeyControlsForControlElementException(info.KeyControls.Length, 1);

        SetKeyAndActionText(info.KeyControls[0], keyText, actionText);
    }
}
