using System.Collections.Generic;
using UnityEngine;

public abstract class SetBaseVisualizerElementColorToTrackColor : BaseVisualizerElement
{
    [SerializeField] private VisualizerColorType colorType;
    public VisualizerColorType ColorType { get { return colorType; } set { colorType = value; } }
    [SerializeField] private int index;
    public int ColorIndex { get { return index; } set { index = value; } }

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
            SetElementToColor(ColorHelper.BlankColor);
            return;
        }
        SetElementToColor(VisualizerManager._Instance.GetColor(colorType, index));
    }

    public override void RecieveTrackInfo(TrackInfo info)
    {
        if (ignoreBroadcasts) return;

        SetElementColor();
    }

    public override void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (ignoreBroadcasts) return;

        // Base version of function handles enabled/disabled
        base.RecieveVisualizerElementsInfo(info);

        // Update color index
        index = info[label].ColorIndex;
        colorType = info[label].ColorType;

        // Update Color
        SetElementColor();
    }

    protected virtual void SetElementVariable() { }
    protected abstract void SetElementToColor(Color c);
}