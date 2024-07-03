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
    [SerializeField] private GradientDropdownMenu gradientDropdownMenu;
    [SerializeField] private FontDropdownMenu fontDropdownMenu;

    [SerializeField] private Button setSolidColorTypeButton;
    [SerializeField] private Button setGradientByTimeColorTypeButton;
    [SerializeField] private Button setGradientByIndexColorTypeButton;

    private bool active;
    private int colorIndex;
    private int fontIndex;
    private VisualizerColorType colorType;

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
        gradientDropdownMenu.OnSelectElement += x =>
        {
            colorIndex = x;
            UpdateColorIndex();
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

    private void SetGradient()
    {
        gradientDropdownMenu.ActivateElement(colorIndex);
    }

    private void SetFont()
    {
        fontDropdownMenu.ActivateElement(fontIndex);
    }

    private void SetColorType()
    {
        setGradientByIndexColorTypeButton.interactable = colorType != VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT;
        setGradientByTimeColorTypeButton.interactable = colorType != VisualizerColorType.TIME_BASED_GRADIENT;
        setSolidColorTypeButton.interactable = colorType != VisualizerColorType.COLOR;

        if (colorType == VisualizerColorType.COLOR)
        {
            colorDropdownMenu.gameObject.SetActive(true);
            gradientDropdownMenu.gameObject.SetActive(false);
            SetColor();
        }
        else if (colorType == VisualizerColorType.TIME_BASED_GRADIENT || colorType == VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT)
        {
            colorDropdownMenu.gameObject.SetActive(false);
            gradientDropdownMenu.gameObject.SetActive(true);
            SetGradient();
        }
    }

    public void UpdateColorType(int enumIndex)
    {
        UpdateColorType((VisualizerColorType)enumIndex);
    }

    public void UpdateColorType(VisualizerColorType type)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorType = type;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
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
        colorType = info[label].ColorType;
        colorIndex = info[label].ColorIndex;
        fontIndex = info[label].FontIndex;
        active = info[label].Enabled;

        SetEnabledButtonColor();

        SetColorType();
        SetFont();
    }
}
