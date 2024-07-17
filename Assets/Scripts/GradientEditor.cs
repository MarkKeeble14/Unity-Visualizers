using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class GradientEditor : MonoBehaviour
{
    public static GradientEditor _Instance { get; private set; }

    [SerializeField] private GameObject gradientEditor;
    [SerializeField] private GradientDisplay gradientDisplay;
    private Gradient currentGradient;

    [SerializeField] private Image selectedKeyColorDisplay;
    [SerializeField] private TMP_InputField positionInputField;

    [SerializeField] private Button deleteKeyButton;
    [SerializeField] private Button keyColorButton;
    [SerializeField] private TMP_InputField keyPositionInput;

    public Action<Gradient> OnGradientFinalized;
    private int currentlyEditingKeyIndex = -1;


    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;
    }

    public void Open(Gradient gradientState)
    {
        currentGradient = gradientState;

        UpdateGradientDisplay();

        SetCurrentlyEditingKeyIndex(-1);

        gradientEditor.SetActive(true);
    }

    public void AddKey(float percentPos)
    {
        // Unity Gradients are limited to 8 keys
        if (currentGradient.colorKeys.Length >= 8)
        {
            Debug.LogWarning("Unable to add any more keys to Gradient");
            return;
        }

        GradientColorKey currentlyEditingKey = new GradientColorKey(Color.white, percentPos);

        // Make new keys
        List<GradientColorKey> newColorKeys = new List<GradientColorKey>() { currentlyEditingKey };
        currentGradient.colorKeys.ToList().ForEach(x => newColorKeys.Add(x));

        // Set keys
        currentGradient.SetKeys(newColorKeys.ToArray(), currentGradient.alphaKeys);

        // Update colors
        gradientDisplay.UpdateColors(currentGradient);
    }

    public void SetEditingKey(int index, Color c, float time)
    {
        SetCurrentlyEditingKeyIndex(index);
        selectedKeyColorDisplay.color = c;
        SetPositionText(time);
    }

    private void SetPositionText(float time)
    {
        positionInputField.text = string.Format("{0}%", Mathf.CeilToInt(time * 100));
    }

    private void SetCurrentlyEditingKeyIndex(int index)
    {
        currentlyEditingKeyIndex = index;
        bool isSentinal = index == -1;
        keyColorButton.interactable = !isSentinal;
        keyPositionInput.interactable = !isSentinal;
        deleteKeyButton.interactable = !isSentinal;
    }

    public void ChangeKeyColor()
    {
        RGBColorPicker._Instance.OnColorFinalized += ColorChosen;

        RGBColorPicker._Instance.Open(selectedKeyColorDisplay.color);
    }

    private void ColorChosen(Color c)
    {
        GradientColorKey[] colorKeys = currentGradient.colorKeys;
        colorKeys[currentlyEditingKeyIndex].color = c;
        currentGradient.SetKeys(colorKeys, currentGradient.alphaKeys);

        selectedKeyColorDisplay.color = c;

        UpdateGradientDisplay();

        RGBColorPicker._Instance.OnColorFinalized -= ColorChosen;
    }

    public void ChangeKeyTime(string s)
    {
        float f;
        if (float.TryParse(s, out f))
        {
            if (f < 0 || f > 1)
            {
                Debug.LogWarning("Attempted to set an invalid time for gradient key");
                return;
            }

            GradientColorKey[] colorKeys = currentGradient.colorKeys;
            colorKeys[currentlyEditingKeyIndex].time = f;
            currentGradient.SetKeys(colorKeys, currentGradient.alphaKeys);

            SetPositionText(f);

            UpdateGradientDisplay();
        }
    }

    public void DeleteSelectedKey()
    {
        List<GradientColorKey> colorKeys = currentGradient.colorKeys.ToList();
        colorKeys.RemoveAt(currentlyEditingKeyIndex);
        currentGradient.SetKeys(colorKeys.ToArray(), currentGradient.alphaKeys);

        UpdateGradientDisplay();
    }

    private void UpdateGradientDisplay()
    {
        gradientDisplay.UpdateColors(currentGradient);
        gradientDisplay.RemakeKeys();
    }

    public void FinalizeGradient()
    {
        OnGradientFinalized(currentGradient);
        Close();
    }

    private void Close()
    {
        gradientDisplay.DestroyKeys();
        gradientEditor.gameObject.SetActive(false);
    }
}
