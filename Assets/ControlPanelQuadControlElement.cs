using TMPro;
using UnityEngine;

public class ControlPanelQuadControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI downText;
    [SerializeField] private TextMeshProUGUI leftText;

    public override void Construct(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 4) throw new System.Exception(); // TODO: Custom Exception

        upText.text = info.KeyControls[0].GetKeyAndActionString();
        rightText.text = info.KeyControls[1].GetKeyAndActionString();
        downText.text = info.KeyControls[2].GetKeyAndActionString();
        leftText.text = info.KeyControls[3].GetKeyAndActionString();
    }
}
