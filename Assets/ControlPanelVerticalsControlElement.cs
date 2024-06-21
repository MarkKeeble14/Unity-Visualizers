using TMPro;
using UnityEngine;

public class ControlPanelVerticalsControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI downText;

    public override void Construct(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 2) throw new System.Exception(); // TODO: Custom Exception

        upText.text = info.KeyControls[0].GetKeyAndActionString();
        downText.text = info.KeyControls[1].GetKeyAndActionString();
    }
}
