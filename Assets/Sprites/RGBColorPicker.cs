using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class RGBColorPicker : MonoBehaviour
{
    [SerializeField, Range(0, 255)] private float r;
    [SerializeField, Range(0, 255)] private float g;
    [SerializeField, Range(0, 255)] private float b;

    [Header("References")]
    [SerializeField] private Image compositeColorDisplay;
    [SerializeField] private Image redColorComponent;
    [SerializeField] private Image greenColorComponent;
    [SerializeField] private Image blueColorComponent;

    [SerializeField] private TMP_InputField redTextField;
    [SerializeField] private TMP_InputField greenTextField;
    [SerializeField] private TMP_InputField blueTextField;

    [SerializeField] private TMP_InputField hexTextField;

    public static RGBColorPicker _Instance { get; private set; }

    public Action<Color> OnColorFinalized;

    [SerializeField] private GameObject picker;

    private bool isDropperSelectActive;


    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;
    }

    private void Start()
    {
        SetDisplayColors();
    }

    private void Update()
    {
        if (isDropperSelectActive)
        {
            // Read Color at mouse coordinate
            Color c = GetColorOfHoveredPixel();
            SetR(c.r);
            SetG(c.g);
            SetB(c.b);

            if (Input.GetMouseButtonDown(0)) 
            { 
                // Set variable
                isDropperSelectActive = false;

                ScreenRenderTextureManager._Instance.RescindRenderToTexRequest();
            }
        }
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
        SetR(startingColor.r);
        SetG(startingColor.g);
        SetB(startingColor.b);

        Open();
    }

    public void Close()
    {
        picker.SetActive(false);
    }

    public void SelectionFinalized()
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
    }

    public Color GetCurrentColorRepresentation()
    {
        return new Color(r, g, b);
    }
}
