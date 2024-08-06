using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlPanelElementType
{
    SINGLE,
    HORIZONTALS,
    VERTICALS,
    QUAD
}

[System.Serializable]
public struct ControlPanelElementInfo
{
    public string Label;
    public KeyControl[] KeyControls;
    public ControlPanelElementType Type;

    public ControlPanelElementInfo(string label, KeyControl[] keyControls, ControlPanelElementType type)
    {
        Label = label;
        KeyControls = keyControls;
        Type = type;
    }
}

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private List<ControlPanelElementInfo> controlPanelElements = new();

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform panel;

    [Header("Prefabs")]
    [SerializeField] private ControlPanelElement single;
    [SerializeField] private ControlPanelElement horizontals;
    [SerializeField] private ControlPanelElement verticals;
    [SerializeField] private ControlPanelElement quad;

    private void Awake()
    {
        ConstructUI();
    }

    private void ConstructUI()
    {
        foreach (ControlPanelElementInfo elementInfo in controlPanelElements)
        {
            ControlPanelElement element = null;
            switch (elementInfo.Type)
            {
                case ControlPanelElementType.SINGLE:
                    element = Instantiate(single, panel);
                    break;
                case ControlPanelElementType.HORIZONTALS:
                    element = Instantiate(horizontals, panel);
                    break;
                case ControlPanelElementType.VERTICALS:
                    element = Instantiate(verticals, panel);
                    break;
                case ControlPanelElementType.QUAD:
                    element = Instantiate(quad, panel);
                    break;
            }
            element.Construct(elementInfo);
        }
    }

    private void OnEnable()
    {
        SetCanvasGroupAlpha(0);
    }

    public void SetCanvasGroupAlpha(float v)
    {
        canvasGroup.alpha = v;
        canvasGroup.blocksRaycasts = (v == 0 ? false : true);
    }
}
