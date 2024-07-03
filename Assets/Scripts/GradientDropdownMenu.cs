using UnityEngine;
using System;

public class GradientDropdownMenu : DropdownMenu, IRecieveTrackInfo
{
    [SerializeField] private GradientDisplay gradientDisplay;
    public Action<Gradient> OnGradientSelected;

    public void RecieveTrackInfo(TrackInfo info)
    {
        Clear();

        foreach (Gradient g in info.Gradients)
        {
            GradientDropdownElement e = (GradientDropdownElement)CreateElementObject();
            e.SetGradient(g);
        }
    }

    protected override void SetElementActive(int index)
    {
        if (index >= dropdownElements.Count) { return; }
        Gradient g = ((GradientDropdownElement)dropdownElements[index]).GetGradient();
        gradientDisplay.UpdateColors(g);
        OnGradientSelected?.Invoke(g);
    }
}
