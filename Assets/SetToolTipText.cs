using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolTipContentType
{
    CUSTOM,
    INTENSITY_DEFAULT_VALUE, 
    INTENSITY_MULTIPLIER,
    RANGE_DEFAULT_VALUE,
    RANGE_MULTIPLIER,
    BAND,
    EMISSION_INTENSITY
}

public class SetToolTipText : MonoBehaviour
{
    [SerializeField] private SpawnToolTip spawnToolTip;
    [SerializeField] private string defaultValue;
    [SerializeField] private ToolTipContentType toolTipContentType;

    // Start is called before the first frame update
    void Start()
    {
        SetToolTipToText(toolTipContentType);
    }

    public void SetToolTipToText(ToolTipContentType type)
    {
        toolTipContentType = type;
        spawnToolTip.SetToolTipText(GetToolTipContent(type));
    }

    private string GetToolTipContent(ToolTipContentType key)
    {
        switch (toolTipContentType)
        {
            case ToolTipContentType.CUSTOM:
                return defaultValue;
            case ToolTipContentType.BAND:
                return "Controls the band that the element responds to";
            case ToolTipContentType.INTENSITY_MULTIPLIER:
                return "Controls the brightness of the light when it recieves a strong signal";
            case ToolTipContentType.INTENSITY_DEFAULT_VALUE:
                return "Controls the minimum brightness of the light";
            case ToolTipContentType.EMISSION_INTENSITY:
                return "Controls the emission intensity of the element";
            case ToolTipContentType.RANGE_MULTIPLIER:
                return "Controls the reach of the light when it recieves a strong signal";
            case ToolTipContentType.RANGE_DEFAULT_VALUE:
                return "Controls the minimum reach of the light";
            default:
                throw new UncaughtSwitchTypeException(typeof(ToolTipContentType), key.ToString());
        }
    }
}
