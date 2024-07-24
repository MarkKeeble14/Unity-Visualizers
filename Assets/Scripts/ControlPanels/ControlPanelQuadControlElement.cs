using TMPro;
using UnityEngine;

public class ControlPanelQuadControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI rightActionText;
    [SerializeField] private TextMeshProUGUI rightKeyText;

    [SerializeField] private TextMeshProUGUI leftActionText;
    [SerializeField] private TextMeshProUGUI leftKeyText;

    [SerializeField] private TextMeshProUGUI upActionText;
    [SerializeField] private TextMeshProUGUI upKeyText;

    [SerializeField] private TextMeshProUGUI downActionText;
    [SerializeField] private TextMeshProUGUI downKeyText;

    protected override void MakeElement(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 4) throw new MalformedKeyControlsForControlElementException(info.KeyControls.Length, 4);

        SetKeyAndActionText(info.KeyControls[0], upActionText, upKeyText);
        SetKeyAndActionText(info.KeyControls[1], rightActionText, rightKeyText);
        SetKeyAndActionText(info.KeyControls[2], downActionText, downKeyText);
        SetKeyAndActionText(info.KeyControls[3], leftActionText, leftKeyText);

    }
}
