using TMPro;
using UnityEngine;

public class ControlPanelVerticalsControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI upActionText;
    [SerializeField] private TextMeshProUGUI upKeyText;

    [SerializeField] private TextMeshProUGUI downActionText;
    [SerializeField] private TextMeshProUGUI downKeyText;

    protected override void MakeElement(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 2) throw new System.Exception(); // TODO: Custom Exception

        SetKeyAndActionText(info.KeyControls[0], upActionText, upKeyText);
        SetKeyAndActionText(info.KeyControls[1], downActionText, downKeyText);
    }
}
