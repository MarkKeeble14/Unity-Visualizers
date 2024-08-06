using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorListElement : ListSelectionElement
{
    [SerializeField] private Image image;

    public void Set(int index, Color c)
    {
        SetIndex(index);
        image.color = c;
    }

    public override void Open()
    {
        RGBColorPicker._Instance.AddColorRequest(new ColorRequest(OnColorSelected, image.color, "Color #" + Index));
    }

    private void OnColorSelected(Color c)
    {
        // Update image color
        image.color = c;

        VisualizerManager._Instance.UpdateTrackColor(Index, c);
    }

    public override void Delete()
    {
        VisualizerManager._Instance.DeleteColor(Index);
    }

    protected override void Randomize()
    {
        OnColorSelected(RandomHelper.GetRandomOpaqueColor());
    }
}
