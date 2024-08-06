using TMPro;
using UnityEngine;

public class ControlPanelHorizontalsControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI rightActionText;
    [SerializeField] private TextMeshProUGUI rightKeyText;

    [SerializeField] private TextMeshProUGUI leftActionText;
    [SerializeField] private TextMeshProUGUI leftKeyText;

    protected override void MakeElement(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 2) throw new MalformedKeyControlsForControlElementException(info.KeyControls.Length, 2);

        SetKeyAndActionText(info.KeyControls[0], leftActionText, leftKeyText);
        SetKeyAndActionText(info.KeyControls[1], rightActionText, rightKeyText);
    }
}
