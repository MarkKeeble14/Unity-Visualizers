using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorListElement : ListSelectionElement
{
    [SerializeField] private Image image;

    public void Set(int index, Color c, bool locked)
    {
        SetIndex(index);
        image.color = c;
        SetLocked(locked);
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
        UIManager._Instance.PopupAreYouSureMessage("Delete this Color?",
            () => VisualizerManager._Instance.DeleteColor(Index), null);
    }

    protected override void Randomize(bool skipConfirmation)
    {
        if (skipConfirmation)
        {
            OnColorSelected(RandomHelper.GetRandomOpaqueColor());
        } else
        {
            UIManager._Instance.PopupAreYouSureMessage("Randomize this Color?",
                () => OnColorSelected(RandomHelper.GetRandomOpaqueColor()), null);
        }
    }
}
