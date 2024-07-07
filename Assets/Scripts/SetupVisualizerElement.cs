using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public abstract class SetupVisualizerElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image enabledButtonDisplay;

    [SerializeField] private ColorDropdownMenu colorDropdownMenu;
    [SerializeField] private GradientDropdownMenu gradientDropdownMenu;
    [SerializeField] private FontDropdownMenu fontDropdownMenu;

    [SerializeField] private Button setSolidColorTypeButton;
    [SerializeField] private Button setGradientByTimeColorTypeButton;
    [SerializeField] private Button setGradientByIndexColorTypeButton;

    protected bool active;
    protected int colorIndex;
    protected int fontIndex;
    protected VisualizerColorType colorType;

    protected abstract void UpdateSettings(VisualizerElementsSettings newSettings);

    protected abstract VisualizerElementsSettings GetElementSettings();

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

    protected void Set()
    {
        SetEnabledButtonColor();
        SetColorType();
        SetFont();
    }

    public void UpdateColorType(int enumIndex)
    {
        UpdateColorType((VisualizerColorType)enumIndex);
    }

    public void UpdateColorType(VisualizerColorType type)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorType = type;

        UpdateSettings(newSettings);
    }

    private void UpdateEnabled(bool b)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.Enabled = b;
        UpdateSettings(newSettings);
    }

    private void UpdateColorIndex()
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorIndex = colorIndex;
        UpdateSettings(newSettings);
    }

    private void UpdateFontIndex()
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.FontIndex = fontIndex;
        UpdateSettings(newSettings);
    }
}
