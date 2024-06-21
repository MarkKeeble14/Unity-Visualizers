using TMPro;
using UnityEngine;

public class ControlPanelHorizontalsControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI leftText;

    public override void Construct(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 2) throw new System.Exception(); // TODO: Custom Exception

        rightText.text = info.KeyControls[0].GetKeyAndActionString();
        leftText.text = info.KeyControls[1].GetKeyAndActionString();
    }
}
