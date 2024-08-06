using UnityEngine;

public class GradientDropdownElement : DropdownElement
{
    [SerializeField] private GradientDisplay gradientDisplay;

    private Gradient gradient;

    public Gradient GetGradient() => gradient;

    public void SetGradient(Gradient g)
    {
        gradient = g;
        gradientDisplay.UpdateColors(g);
    }
}
