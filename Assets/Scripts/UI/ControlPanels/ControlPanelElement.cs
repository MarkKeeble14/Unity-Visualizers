using TMPro;
using UnityEngine;

public abstract class ControlPanelElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Construct(ControlPanelElementInfo info)
    {
        // Enable/Disable Label
        if (string.IsNullOrEmpty(info.Label))
            label.gameObject.SetActive(false);
        else
            label.text = info.Label;

        // Make element
        MakeElement(info);
    }

    protected abstract void MakeElement(ControlPanelElementInfo info);

    protected void SetKeyAndActionText(KeyControl key, TextMeshProUGUI keyText, TextMeshProUGUI actionText)
    {
        keyText.text = key.GetKey();
        actionText.text = key.GetAction();
    }
}
