using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TrackInfo
{
    public string Title;
    public Sprite CoverArt;
    public string Duration;
    public Color DominantColor;
    public Color SecondaryColor;
    public Color TertiaryColor;
    public Gradient Gradient;

    public TrackInfo(string trackName, Sprite trackArt, string duration, Color dominantColor, Color secondaryColor, Color tertiaryColor, Gradient gradient) : this()
    {
        Title = trackName;
        CoverArt = trackArt;
        Duration = duration;
        DominantColor = dominantColor;
        SecondaryColor = secondaryColor;
        TertiaryColor = tertiaryColor;
        Gradient = gradient;
    }
}