using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ColorListElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private Image image;

    private int index;
    public int Index => index;

    public void Set(int index, Color c)
    {
        this.index = index;
        image.color = c;
        indexText.text = index.ToString();
    }

    public void OpenColorSelection()
    {
        Debug.Log("Opening Color Selection for Color #" + index);

        RGBColorPicker._Instance.Open(image.color);
        RGBColorPicker._Instance.OnColorFinalized += OnColorSelected;
    }

    private void OnColorSelected(Color c)
    {
        // Update image color
        image.color = c;

        VisualizerManager._Instance.UpdateTrackColor(index, c);

        // Remove callback
        RGBColorPicker._Instance.OnColorFinalized -= OnColorSelected;
    }
}
