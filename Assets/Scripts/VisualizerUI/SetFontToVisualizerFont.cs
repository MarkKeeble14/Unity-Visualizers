using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SetFontToVisualizerFont : VisualizerElement
{
    [SerializeField] private int index;
    [SerializeField] private TextMeshProUGUI text;

    public int FontIndex { get { return index; } set { index = value; } }

    private void SetElementFont()
    {
        text.font = VisualizerManager._Instance.GetFont(index);
    }

    public override void RecieveTrackInfo(TrackInfo info)
    {
        SetElementFont();
    }

    public override void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        // Base version of function handles enabled/disabled
        base.RecieveVisualizerElementsInfo(info);

        // Update color index
        index = info[label].FontIndex;

        // Update Color
        SetElementFont();
    }
}