using UnityEngine;

public abstract class VisualizerElement : MonoBehaviour, IRecieveTrackInfo
{
    private bool active = true;
    public bool Active { get { return active; } set { active = value; } }

    public abstract void RecieveTrackInfo(TrackInfo info);
}
