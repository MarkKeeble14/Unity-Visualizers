using System;
using TMPro;
using UnityEngine;

public abstract class VisualizerSetting : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] protected string key;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private SetToolTipText setToolTipText;

    private void Awake()
    {
        setToolTipText = GetComponent<SetToolTipText>();
    }

    private void Start()
    {
        SetLabelText();
        Initialize();
    }

    protected abstract void Initialize();

    public void SetToolTip(ToolTipContentType toolTipOnHover)
    {
        setToolTipText.SetToolTipToText(toolTipOnHover);
    }

    public void SetLabel(string displayLabel)
    {
        label = displayLabel;
        SetLabelText();
    }

    public void SetKey(string key)
    {
        this.key = key;
    }

    private void SetLabelText()
    {
        labelText.text = label;
    }
}
