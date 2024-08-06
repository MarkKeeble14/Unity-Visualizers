using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class TextDropdownMenu : DropdownMenu
{
    public Action<int> OnTextSelected;

    public void PopulateDropdown(string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            TextDropdownElement e = (TextDropdownElement)CreateElementObject();
            e.SetText(options[i]);
            e.SetIndex(i);
        }
    }

    protected override void SetElementActive(int index)
    {
        labelText.text = ((TextDropdownElement)dropdownElements[index]).GetText();
        OnTextSelected?.Invoke(index);
    }
}
