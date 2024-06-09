using UnityEngine;

public abstract class SetElementColorToMatchTrack : MonoBehaviour, IRecieveTrackInfo
{
    [SerializeField] private VisualizerColorType match;
    private VisualizerColorType lastMatch;
    private bool useGradient;
    protected Gradient trackGradient = null;
    protected Color dominant;
    protected Color secondary;
    protected Color tertiary;

    protected abstract void SetElementVariable();

    protected abstract void SetElementColorByGradient(float percent);
    protected abstract void SetElementToDominantColor();
    protected abstract void SetElementToSecondaryColor();
    protected abstract void SetElementToTertiaryColor();

    private void Awake()
    {
        SetElementVariable();
    }

    private void Update()
    {
        // Update color if need be
        if (match != lastMatch)
            SetElementColor();
        lastMatch = match;

        // Evaluate gradient if need be
        if (!useGradient) return;
        SetElementColorByGradient(VisualizerManager._Instance.PlaythroughPercent);
    }

    private void SetElementColor()
    {
        switch (match)
        {
            case VisualizerColorType.DOMINANT_COLOR:
                useGradient = false;
                SetElementToDominantColor();
                break;
            case VisualizerColorType.SECONDARY_COLOR:
                useGradient = false;
                SetElementToSecondaryColor();
                break;
            case VisualizerColorType.TERTIARY_COLOR:
                useGradient = false;
                SetElementToTertiaryColor();
                break;
            case VisualizerColorType.GRADIENT:
                useGradient = true;
                break;
        }
    }

    public void RecieveTrackInfo(TrackInfo info)
    {
        dominant = info.DominantColor;
        secondary = info.SecondaryColor;
        tertiary = info.TertiaryColor;
        trackGradient = info.Gradient;

        SetElementColor();
    }
}