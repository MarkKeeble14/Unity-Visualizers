using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public abstract class SetupVisualizerElement : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform extraSettingsHolder;
    [SerializeField] private RectTransform myRect;

    [Header("Display Info")]
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image enabledButtonDisplay;

    [Header("Dropdowns")]
    [SerializeField] private ColorDropdownMenu colorDropdownMenu;
    [SerializeField] private GradientDropdownMenu gradientDropdownMenu;
    [SerializeField] private FontDropdownMenu fontDropdownMenu;

    [Header("Set Color Type Buttons")]
    [SerializeField] private Button setSolidColorTypeButton;
    [SerializeField] private Button setGradientByTimeColorTypeButton;
    [SerializeField] private Button setGradientByIndexColorTypeButton;

    [Header("Other Settings")]
    [SerializeField] private float extraSettingHeight = 30;
    private float expandedHeight;
    private float defaultHeight;
    private bool expanded;

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

        // set height variables
        defaultHeight = myRect.sizeDelta.y;

        float extraSettingsHeight = extraSettingsHolder.childCount * extraSettingHeight;
        expandedHeight = myRect.sizeDelta.y + extraSettingsHeight;

        // set height of settings
        RectTransform extraSettingsRect = extraSettingsHolder.GetComponent<RectTransform>();
        Vector2 sizeDelta = extraSettingsRect.sizeDelta;
        sizeDelta.y = extraSettingsHeight;
        extraSettingsRect.sizeDelta = sizeDelta;

        foreach (Transform child in extraSettingsHolder)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            sizeDelta = childRect.sizeDelta;
            sizeDelta.y = extraSettingHeight;
            childRect.sizeDelta = sizeDelta;
        }

        // ensure extra settings are hidden
        extraSettingsHolder.gameObject.SetActive(false);
    }

    public void ToggleExpandedSize()
    {
        if (extraSettingsHolder.childCount == 0) return;

        expanded = !expanded;
        Vector2 sizeDelta = myRect.sizeDelta;
        if (expanded)
        {
            sizeDelta.y = expandedHeight;
        }
        else
        {
            sizeDelta.y = defaultHeight;
        }
        myRect.sizeDelta = sizeDelta;
        extraSettingsHolder.gameObject.SetActive(expanded);
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
