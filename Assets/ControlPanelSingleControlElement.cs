using TMPro;
using UnityEngine;

public class ControlPanelSingleControlElement : ControlPanelElement
{
    [SerializeField] private TextMeshProUGUI text;

    public override void Construct(ControlPanelElementInfo info)
    {
        if (info.KeyControls.Length != 1) throw new System.Exception(); // TODO: Custom Exception

        text.text = info.KeyControls[0].GetKeyAndActionString();
    }
}
