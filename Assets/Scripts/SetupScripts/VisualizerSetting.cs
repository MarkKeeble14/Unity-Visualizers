using System;
using TMPro;
using UnityEngine;

public abstract class VisualizerSetting : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] protected string key;

    [Header("References")]
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

    public void SetToolTip(SettingType toolTipOnHover)
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

    public void SetHeight(float height)
    {
        RectTransform rectTransform = transform as RectTransform;
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = height;
        rectTransform.sizeDelta = sizeDelta;
    }
}
