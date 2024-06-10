using UnityEngine;

public abstract class SetElementColorToMatchTrack : MonoBehaviour, IRecieveTrackInfo
{
    [SerializeField] private VisualizerColorType colorType;
    [SerializeField] private int colorIndex;

    protected abstract void SetElementVariable();
    protected abstract void SetElementToColor(Color c);

    private void Awake()
    {
        SetElementVariable();
    }

    private void Update()
    {
        SetElementColor();
    }

    private void SetElementColor()
    {
        SetElementToColor(VisualizerManager._Instance.GetColor(colorType, colorIndex, Vector2.zero));
    }

    public void RecieveTrackInfo(TrackInfo info)
    {
        SetElementColor();
    }
}