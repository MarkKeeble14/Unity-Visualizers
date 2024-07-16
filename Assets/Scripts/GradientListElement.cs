using UnityEngine;
using UnityEngine.UI;

public class GradientListElement : ListSelectionElement
{
    [SerializeField] private GradientDisplay display;
    [SerializeField] private Gradient gradient;

    public void Set(int index, Gradient g)
    {
        SetIndex(index);
        gradient = g;
        display.UpdateColors(g);
    }

    public override void OpenListSelection()
    {
        GradientEditor._Instance.EditGradient(gradient, gradient => VisualizerManager._Instance.UpdateTrackGradient(Index, gradient));
    }
}
