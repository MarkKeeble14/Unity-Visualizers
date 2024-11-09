using UnityEngine;
using UnityEngine.UI;

public class GradientListElement : ListSelectionElement
{
    [SerializeField] private GradientDisplay display;
    [SerializeField] private Gradient gradient;

    public void Set(int index, Gradient g, bool locked)
    {
        SetIndex(index);
        gradient = g;
        display.UpdateColors(g);
        SetLocked(locked);
    }

    public override void Open()
    {
        GradientEditor._Instance.Open(gradient);

        GradientEditor._Instance.OnGradientFinalized += OnGradientSelected;
    }

    private void OnGradientSelected(Gradient g)
    {
        gradient = g;

        // Update image color
        display.UpdateColors(gradient);

        VisualizerManager._Instance.UpdateTrackGradient(Index, gradient);

        // Remove callback
        GradientEditor._Instance.OnGradientFinalized -= OnGradientSelected;
    }

    public override void Delete()
    {
        UIManager._Instance.PopupAreYouSureMessage("Delete this Gradient?",
            () => VisualizerManager._Instance.DeleteGradient(Index), null);
    }

    protected override void Randomize(bool skipConfirmation)
    {
        if (skipConfirmation)
        {
            gradient = RandomHelper.GetRandomOpaqueGradient();

            // Update image color
            display.UpdateColors(gradient);

            VisualizerManager._Instance.UpdateTrackGradient(Index, gradient);
        }
        else
        {
            UIManager._Instance.PopupAreYouSureMessage("Randomize this Gradient?",
                () =>
                {
                    gradient = RandomHelper.GetRandomOpaqueGradient();

                    // Update image color
                    display.UpdateColors(gradient);

                    VisualizerManager._Instance.UpdateTrackGradient(Index, gradient);
                }, null);
        }
    }
}
