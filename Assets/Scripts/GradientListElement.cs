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
        GradientEditor._Instance.Open(gradient);
        GradientEditor._Instance.OnGradientFinalized += OnGradientSelected;
    }

    private void OnGradientSelected(Gradient g)
    {
        // Update image color
        display.UpdateColors(g);

        VisualizerManager._Instance.UpdateTrackGradient(Index, gradient);

        // Remove callback
        GradientEditor._Instance.OnGradientFinalized -= OnGradientSelected;
    }
}
