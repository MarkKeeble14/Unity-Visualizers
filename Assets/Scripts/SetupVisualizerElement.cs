using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupVisualizerElement : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;

    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image enabledButtonDisplay;

    [SerializeField] private ColorDropdownMenu colorDropdownMenu;
    [SerializeField] private FontDropdownMenu fontDropdownMenu;

    private bool active;
    private int colorIndex;
    private int fontIndex;

    private void Awake()
    {
        colorDropdownMenu.OnSelectElement += x =>
        {
            colorIndex = x;
            UpdateColorIndex();
        };
        fontDropdownMenu.OnSelectElement += x =>
        {
            fontIndex = x;
            UpdateFontIndex();
        };
    }

    public void ToggleActive()
    {
        active = !active;
        UpdateEnabled(active);
    }

    private void SetEnabledButtonColor()
    {
        enabledButtonDisplay.color = (active ? Color.green : Color.red);
    }

    private void SetColor()
    {
        colorDropdownMenu.ActivateElement(colorIndex);
    }

    private void SetFont()
    {
        fontDropdownMenu.ActivateElement(fontIndex);
    }

    private void UpdateEnabled(bool b)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.Enabled = b;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    private void UpdateColorIndex()
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorIndex = colorIndex;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    private void UpdateFontIndex()
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.FontIndex = fontIndex;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    private VisualizerElementsSettings GetElementSettings()
    {
        return VisualizerManager._Instance.GetVisualizerElementSettings(label);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        colorIndex = info[label].ColorIndex;
        fontIndex = info[label].FontIndex;
        active = info[label].Enabled;

        SetEnabledButtonColor();
        SetColor();
        SetFont();
    }
}
