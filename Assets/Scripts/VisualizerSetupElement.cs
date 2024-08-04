using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public struct SetupElementHelperButtonInfo
{
    public UnityEvent OnPress;
    public UnityEvent<Button> ControlInteractable;
    public string Label;
}

[System.Serializable]
public struct SetupElementInfo
{
    public VisualizerElementLabel Label;
    public VisualizerElementsSettings DefaultSettings;
    public string Name;
    public bool AllowSolidColor;
    public bool AllowTimeBasedGradient;
    public bool AllowPositionBasedGradient;
    public bool AllowColorSelection;
    public bool AllowFontSelection;
    public SetupElementHelperButtonInfo[] HelperButtons;

    public SetupElementInfo(VisualizerElementLabel label, VisualizerElementsSettings defaultSettings, string name, bool allowFontSelection, bool allowColorSelection,
        bool allowSolidColor, bool allowTimeBasedGradient, bool allowPositionBasedGradient, SetupElementHelperButtonInfo[] helperButtons)
    {
        Label = label;
        DefaultSettings = defaultSettings;
        Name = name;
        AllowFontSelection = allowFontSelection;
        AllowColorSelection = allowColorSelection;
        AllowSolidColor = allowSolidColor;
        AllowTimeBasedGradient = allowTimeBasedGradient;
        AllowPositionBasedGradient = allowPositionBasedGradient;
        HelperButtons = helperButtons;
    }
}

public class VisualizerSetupElement : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;

    [SerializeField] private GameObject fontSelection;
    [SerializeField] private GameObject colorSelection;
    [SerializeField] private VisualizerSetupHelperButton[] helperButtons;

    [Header("Display Info")]
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Checkbox enabledCheckbox;

    [Header("Dropdowns")]
    [SerializeField] private ColorDropdownMenu colorDropdownMenu;
    [SerializeField] private GradientDropdownMenu gradientDropdownMenu;
    [SerializeField] private FontDropdownMenu fontDropdownMenu;

    [Header("Set Color Type Buttons")]
    [SerializeField] private Button setSolidColorTypeButton;
    [SerializeField] private Button setTimeBasedGradientButton;
    [SerializeField] private Button setPositionBasedGradientButton;

    [Header("Other Settings")]
    [SerializeField] private float extraSettingHeight = 30;
    [SerializeField] private float expandedHeightAdjustment = 20;

    [Header("Transforms")]
    [SerializeField] private Transform extraSettingsHolder;
    private RectTransform rect => transform as RectTransform;
    public Transform ExtraSettingsHolder => extraSettingsHolder;

    protected bool active = true;
    private float expandedHeight;
    private float defaultHeight;
    private bool expanded;
    protected int colorIndex;
    protected int fontIndex;
    protected VisualizerColorType colorType;

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

        // set variables
        defaultHeight = rect.sizeDelta.y;

        CalcLayout();
    }

    public void Init(SetupElementInfo info)
    {
        labelText.text = info.Name;
        label = info.Label;

        fontSelection.SetActive(info.AllowFontSelection);
        colorSelection.SetActive(info.AllowColorSelection);
        setSolidColorTypeButton.gameObject.SetActive(info.AllowSolidColor);
        setTimeBasedGradientButton.gameObject.SetActive(info.AllowTimeBasedGradient);
        setPositionBasedGradientButton.gameObject.SetActive(info.AllowPositionBasedGradient);

        for (int i = 0; i < info.HelperButtons.Length; i++)
        {
            VisualizerSetupHelperButton b = helperButtons[i];
            b.gameObject.SetActive(true);
            b.ControlButtonInteractable = button => info.HelperButtons[b.Index].ControlInteractable?.Invoke(button);
            b.Button.onClick.AddListener(() => { info.HelperButtons[b.Index].OnPress?.Invoke(); });
            b.Text.text = info.HelperButtons[i].Label;
        }
    }

    private void CalcLayout()
    {
        float extraSettingsHeight = extraSettingsHolder.childCount * extraSettingHeight;
        expandedHeight = rect.sizeDelta.y + extraSettingsHeight + expandedHeightAdjustment;

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
        Vector2 sizeDelta = rect.sizeDelta;
        if (expanded)
        {
            sizeDelta.y = expandedHeight;
        }
        else
        {
            sizeDelta.y = defaultHeight;
        }
        rect.sizeDelta = sizeDelta;
        extraSettingsHolder.gameObject.SetActive(expanded);
    }

    public void ToggleActive()
    {
        active = !active;
        UpdateEnabled(active);
    }

    private void SetColorType()
    {
        setPositionBasedGradientButton.interactable = colorType != VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT;
        setTimeBasedGradientButton.interactable = colorType != VisualizerColorType.TIME_BASED_GRADIENT;
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
        colorDropdownMenu.ActivateElementAtIndex(colorIndex);
    }

    private void SetGradient()
    {
        gradientDropdownMenu.ActivateElementAtIndex(colorIndex);
    }

    private void SetFont()
    {
        fontDropdownMenu.ActivateElementAtIndex(fontIndex);
    }

    private void SetCheckbox()
    {
        enabledCheckbox.Active = active;
    }

    protected void Set()
    {
        SetCheckbox();
        SetColorType();
        SetFont();
    }

    public void Randomize()
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.FontIndex = RandomHelper.RandomIntExclusive(0, VisualizerManager._Instance.TrackInfo.Fonts.Count);
        newSettings.ColorType = (VisualizerColorType)RandomHelper.RandomIntExclusive(0, 2);
        if (newSettings.ColorType == VisualizerColorType.COLOR)
        {
            newSettings.ColorIndex = RandomHelper.RandomIntExclusive(0, VisualizerManager._Instance.TrackInfo.Colors.Count);
        } else if (newSettings.ColorType == VisualizerColorType.TIME_BASED_GRADIENT 
            || newSettings.ColorType == VisualizerColorType.POSITIONAL_INDEX_BASED_GRADIENT)
        {
            newSettings.ColorIndex = RandomHelper.RandomIntExclusive(0, VisualizerManager._Instance.TrackInfo.Gradients.Count);
        }
        UpdateSettings(newSettings);
    }

    public void UpdateColorType(int enumIndex)
    {
        UpdateColorType((VisualizerColorType)enumIndex);
    }

    public void UpdateColorType(VisualizerColorType type)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorType = type;
        newSettings.ColorIndex = 0;

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

    private VisualizerElementsSettings GetElementSettings()
    {
        return VisualizerManager._Instance.GetVisualizerElementSettings(label);
    }

    private void UpdateSettings(VisualizerElementsSettings newSettings)
    {
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            colorType = x.ColorType;
            colorIndex = x.ColorIndex;
            fontIndex = x.FontIndex;
            active = x.Enabled;

            Set();
        });
    }
}
