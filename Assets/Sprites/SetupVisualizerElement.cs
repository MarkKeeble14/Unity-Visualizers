using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupVisualizerElement : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;

    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image colorDisplay;
    [SerializeField] private Image enabledButtonDisplay;
    [SerializeField] private TMP_InputField colorIndexInput;
    [SerializeField] private TMP_InputField fontIndexInput;
    private bool active;
    private int colorIndex;
    private int fontIndex;

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
        colorIndexInput.text = colorIndex.ToString();
        colorDisplay.color = VisualizerManager._Instance.GetColor(VisualizerColorType.COLOR, colorIndex);
    }

    private void SetFont()
    {
        fontIndexInput.text = fontIndex.ToString();
        labelText.font = VisualizerManager._Instance.GetFont(fontIndex);
    }

    private int ParseStringForInt(string str)
    {
        int i;
        if (int.TryParse(str, out i))
        {
            return i;
        }
        return -1;
    }

    public void UpdateColorIndex(string s)
    {
        int i = ParseStringForInt(s);
        if (i != -1)
        {
            UpdateColorIndex(i);
        }
    }

    public void UpdateFontIndex(string s)
    {
        int i = ParseStringForInt(s);
        if (i != -1)
        {
            UpdateFontIndex(i);
        }
    }

    private void UpdateEnabled(bool b)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.Enabled = b;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    private void UpdateColorIndex(int i)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.ColorIndex = i;
        VisualizerManager._Instance.UpdateVisualizerElementSettings(label, newSettings);
    }

    private void UpdateFontIndex(int i)
    {
        VisualizerElementsSettings newSettings = GetElementSettings();
        newSettings.FontIndex = i;
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
