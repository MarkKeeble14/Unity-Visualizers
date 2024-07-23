using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class RGBColorPicker : MonoBehaviour
{
    [Header("Numbers")]
    [SerializeField, Range(0, 255)] private float r;
    [SerializeField, Range(0, 255)] private float g;
    [SerializeField, Range(0, 255)] private float b;

    [SerializeField] private float inDropperSelectionHeight = 800;
    [SerializeField] private float defaultHeight = 400;
    [SerializeField] private float offsetFromBorders = 50;

    [Header("References")]
    [SerializeField] private CanvasGroup rgbPickerCanvasGroup;
    [SerializeField] private Image compositeColorDisplay;
    [SerializeField] private Image redColorComponent;
    [SerializeField] private Image greenColorComponent;
    [SerializeField] private Image blueColorComponent;
    [SerializeField] private GameObject picker;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField redTextField;
    [SerializeField] private TMP_InputField greenTextField;
    [SerializeField] private TMP_InputField blueTextField;
    [SerializeField] private TMP_InputField hexTextField;

    [Header("Color Texture")]
    [SerializeField] private RectTransform colorPickerTransform;
    [SerializeField] private GameObject colorTexture;

    [Header("Alt Color Display")]
    [SerializeField] private Image altCompositeColorDisplay;
    [SerializeField] private GameObject altColorDisplayContainer;

    public static RGBColorPicker _Instance { get; private set; }

    public Action<Color> OnColorFinalized;

    private bool isDropperSelectActive;

    public bool IsPickerHidden => rgbPickerCanvasGroup.alpha == 0;
    private Color hoveredPixelColor;
    private Color openColor;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;
    }

    private void Start()
    {
        SetColorPickerHeight(defaultHeight);
        SetDisplayColors();
    }


    private void Update()
    {
        if (redTextField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            greenTextField.Select();
        }
        if (greenTextField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            blueTextField.Select();
        }
        if (blueTextField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            hexTextField.Select();
        }

        if (isDropperSelectActive)
        {
            // Read Color at mouse coordinate
            hoveredPixelColor = GetColorOfHoveredPixel();
            SetR(hoveredPixelColor.r);
            SetG(hoveredPixelColor.g);
            SetB(hoveredPixelColor.b);

            if (Input.GetMouseButtonDown(0))
            {
                DisableDropperSelection();
            }
        }

        if (IsPickerHidden) { altCompositeColorDisplay.color = hoveredPixelColor; }
    }

    private Color GetColorOfHoveredPixel()
    {
        Texture2D tex = ScreenRenderTextureManager._Instance.Tex;
        return (tex == null ? Color.white : ScreenRenderTextureManager._Instance.Tex.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y));
    }

    public void Open()
    {
        picker.SetActive(true);
    }

    public void Open(Color startingColor)
    {
        openColor = startingColor;
        SetR(startingColor.r);
        SetG(startingColor.g);
        SetB(startingColor.b);
        Open();
    }

    public void Close()
    {
        picker.SetActive(false);
    }

    public void Cancel()
    {
        OnColorFinalized?.Invoke(openColor);
        Close();
    }

    public void FinalizeColor()
    {
        OnColorFinalized?.Invoke(GetCurrentColorRepresentation());
        Close();
    }

    public void ParseHexadecimalColor(string hexString)
    {
        Color c;
        if (ColorUtility.TryParseHtmlString(hexString, out c))
        {
            Debug.Log("Successfully Parsed Hexadecimal Color");
            SetR(c.r);
            SetG(c.g);
            SetB(c.b);

            SetDisplayColors();
        }
    }

    public void SetR(string num) { SetComponentValue(num, x => SetR(x / 255)); }
    public void SetG(string num) { SetComponentValue(num, x => SetG(x /255)); }
    public void SetB(string num) { SetComponentValue(num, x => SetB(x / 255)); }

    public void SetR(float r)
    {
        this.r = r;
        redTextField.text = (r * 255).ToString();

        SetDisplayColors();
    }

    public void SetG(float g)
    {
        this.g = g;
        greenTextField.text = (g * 255).ToString();

        SetDisplayColors();
    }

    public void SetB(float b)
    {
        this.b = b;
        blueTextField.text = (b * 255).ToString();

        SetDisplayColors();
    }

    public void SetComponentValue(string num, Action<float> settingFunc)
    {
        float v;
        if (float.TryParse(num, out v))
        {
            settingFunc(v);
        }
    }

    public void SetDisplayColors()
    {
        redColorComponent.color = new Color(r, 0, 0);
        greenColorComponent.color = new Color(0, b, 0);
        blueColorComponent.color = new Color(0, 0, g);
        compositeColorDisplay.color = GetCurrentColorRepresentation();
        hexTextField.text = "#" + ColorUtility.ToHtmlStringRGB(compositeColorDisplay.color);
    }

    public void ActivateDropperSelection()
    {
        // Set variable
        isDropperSelectActive = true;

        ScreenRenderTextureManager._Instance.RequestRenderToTex();

        SetColorPickerHeight(inDropperSelectionHeight);

        colorTexture.SetActive(true);
    }

    private void DisableDropperSelection()
    {
        // Set variable
        isDropperSelectActive = false;

        ScreenRenderTextureManager._Instance.RescindRenderToTexRequest();

        SetColorPickerHeight(defaultHeight);

        colorTexture.SetActive(false);
    }

    public void HideRGBPicker()
    {
        rgbPickerCanvasGroup.alpha = 0;
        rgbPickerCanvasGroup.blocksRaycasts = false;

        altColorDisplayContainer.SetActive(true);
    }

    public void ShowRGBPicker()
    {
        rgbPickerCanvasGroup.alpha = 1;
        rgbPickerCanvasGroup.blocksRaycasts = true;

        altColorDisplayContainer.SetActive(false);
    }


    private void SetColorPickerHeight(float height)
    {
        Vector2 sizeDelta = colorPickerTransform.sizeDelta;
        sizeDelta.y = height;
        colorPickerTransform.sizeDelta = sizeDelta;

        Vector2 anchoredPosition = colorPickerTransform.anchoredPosition;
        anchoredPosition.y = (height / 2) + offsetFromBorders;
        colorPickerTransform.anchoredPosition = anchoredPosition;
    }

    public Color GetCurrentColorRepresentation()
    {
        return new Color(r, g, b);
    }
}
