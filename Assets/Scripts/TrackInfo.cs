using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TrackInfo
{
    public string Title;
    public Sprite CoverArt;
    public string Duration;

    public TrackInfo(string trackName, Sprite trackArt, string duration) : this()
    {
        Title = trackName;
        CoverArt = trackArt;
        Duration = duration;
    }
}