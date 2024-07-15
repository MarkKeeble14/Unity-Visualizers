using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TrackInfo
{
    public string Title;
    public Sprite CoverArt;
    public AudioClip AudioClip;
    public List<Color> Colors;
    public List<Gradient> Gradients;
    public List<TMP_FontAsset> Fonts;
    public string Duration;

    public TrackInfo(string trackName, Sprite trackArt, string duration, AudioClip audioClip, 
        List<Color> colors, List<Gradient> gradients, List<TMP_FontAsset> fonts) : this()
    {
        Title = trackName;
        CoverArt = trackArt;
        Duration = duration;
        AudioClip = audioClip;
        Colors = colors;
        Gradients = gradients;
        Fonts = fonts;
    }
}