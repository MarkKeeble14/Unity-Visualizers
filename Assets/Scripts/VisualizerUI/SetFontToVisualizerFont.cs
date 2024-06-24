using UnityEngine;
using TMPro;

public class SetFontToVisualizerFont : VisualizerElement
{
    [SerializeField] private int index;
    [SerializeField] private TextMeshProUGUI text;

    public int FontIndex { get { return index; } set { index = value; } }

    public override void RecieveTrackInfo(TrackInfo info)
    {
        text.font = VisualizerManager._Instance.GetFont(index);
    }
}