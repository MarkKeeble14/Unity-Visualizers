using UnityEngine;
using TMPro;

[System.Serializable]
public struct VisualizerSelectionInfo
{
    public string ScenarioName;
    public Sprite ScenarioBackgroundSprite;
    public string LoadSceneName;
    public Color TextColor;
    public TMP_FontAsset TextFont;
    public Color BorderColor;
    public Color ButtonHoveredColor;
    public Color ButtonPressedColor;
    public AudioClip OnSelectSound;
    public string TransitionName;
}
