using UnityEngine;

public abstract class SetElementColorToMatchTrack : VisualizerElement
{
    [SerializeField] private VisualizerColorType colorType;
    [SerializeField] private int colorIndex;

    public int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
    public VisualizerColorType ColorType { get { return colorType; } set { colorType = value; } }

    private Color blankColor = new Color(0, 0, 0, 0);

    protected virtual void Awake()
    {
        SetElementVariable();
    }

    protected virtual void Update()
    {
        SetElementColor();
    }

    private void SetElementColor()
    {
        if (!Active)
        {
            SetElementToColor(blankColor);
            return;
        }
        SetElementToColor(VisualizerManager._Instance.GetColor(colorType, colorIndex, Vector2.zero));
    }

    public override void RecieveTrackInfo(TrackInfo info)
    {
        SetElementColor();
    }

    protected virtual void SetElementVariable() { }
    protected abstract void SetElementToColor(Color c);
}