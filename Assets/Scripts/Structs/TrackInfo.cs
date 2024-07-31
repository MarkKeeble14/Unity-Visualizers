using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TrackInfo
{
    public string Title;
    public Sprite CoverArt;
    public Sprite Background;
    public AudioClip AudioClip;
    public List<Color> Colors;
    public List<Gradient> Gradients;
    public List<TMP_FontAsset> Fonts;
    public string DurationString
    {
        get
        {
            return durationString;
        }
        set
        {
            durationString = value;
            Duration = StringHelper.GetDurationFromText(value);
        }
    }
    public float Duration { get; private set; }
    [SerializeField] private string durationString;

    public TrackInfo(string trackName, Sprite trackArt, Sprite background, string duration, AudioClip audioClip, 
        List<Color> colors, List<Gradient> gradients, List<TMP_FontAsset> fonts) : this()
    {
        Title = trackName;
        CoverArt = trackArt;
        Background = background;
        DurationString = duration;
        AudioClip = audioClip;
        Colors = colors;
        Gradients = gradients;
        Fonts = fonts;
    }
}