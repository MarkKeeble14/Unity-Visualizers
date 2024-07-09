using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class VisualizerAttachmentTypeDropdownMenu : DropdownMenu
{
    public Action<AttachmentType> OnAttachmentSelected;

    protected override void SetElementActive(int index)
    {
        // create dropdown elements if not done yet
        if (dropdownElements.Count == 0)
        {
            foreach (AttachmentType t in Enum.GetValues(typeof(AttachmentType)))
            {
                TextDropdownElement e = (TextDropdownElement)CreateElementObject();
                e.SetText(t.ToString());
            }
        }

        labelText.text = ((TextDropdownElement)dropdownElements[index]).GetText();
        OnAttachmentSelected?.Invoke((AttachmentType)index);
    }
}
