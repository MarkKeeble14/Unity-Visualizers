using System.Collections.Generic;
using UnityEngine;

public abstract class VisualizerElement : MonoBehaviour, IRecieveTrackInfo, IRecieveVisualizerElementsInfo
{
    [SerializeField] protected VisualizerElementLabel label;
    private bool active = true;
    public bool Active { get { return active; } set { active = value; } }

    public virtual void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (info.ContainsKey(label))
        {
            active = info[label].Enabled;
            OnActiveSet(active);
        }
    }

    public abstract void RecieveTrackInfo(TrackInfo info);

    protected virtual void OnActiveSet(bool active) { }
}
