using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image colorDisplay;
    [SerializeField] private Image enabledButtonDisplay;
    private bool active = true;
    private int colorIndex;
    private int fontIndex;

    public int ColorIndex => colorIndex;
    public int FontIndex => fontIndex;
    public bool Active => active;

    private void Start()
    {
        SetEnabledButtonColor();
        SetColor();
        SetFont();
    }

    public void ToggleActive()
    {
        active = !active;
        SetEnabledButtonColor();
    }

    private void SetEnabledButtonColor()
    {
        enabledButtonDisplay.color = (active ? Color.green : Color.red);
    }

    public void UpdateColor(string colorId)
    {
        int i;
        if (int.TryParse(colorId, out i))
        {
            colorIndex = i;
            SetColor();
        }
    }

    private void SetColor()
    {
        colorDisplay.color = VisualizerManager._Instance.GetColor(VisualizerColorType.COLOR, colorIndex);
    }

    public void UpdateFont(string fontId)
    {
        int i;
        if (int.TryParse(fontId, out i))
        {
            fontIndex = i;
            SetFont();
        }
    }

    private void SetFont()
    {
        labelText.font = VisualizerManager._Instance.GetFont(fontIndex);
    }
}
