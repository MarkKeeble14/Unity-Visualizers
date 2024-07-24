using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class VisualizerSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scenarioNameText;
    [SerializeField] private Image scenarioImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform textHolder;
    private VisualizerSelectionInfo sceneInfo;
    
    public void SetSceneInfo(VisualizerSelectionInfo sceneInfo)
    {
        this.sceneInfo = sceneInfo;

        // Set info
        scenarioImage.sprite = sceneInfo.ScenarioBackgroundSprite;
        scenarioNameText.text = sceneInfo.ScenarioName;

        // Set aesthetics
        scenarioNameText.color = sceneInfo.TextColor;
        scenarioNameText.font = sceneInfo.TextFont;

        borderImage.color = sceneInfo.BorderColor;

        // Set Button colors
        ColorBlock buttonColors = button.colors;
        buttonColors.pressedColor = sceneInfo.ButtonPressedColor;
        buttonColors.highlightedColor = sceneInfo.ButtonHoveredColor;
        button.colors = buttonColors;
    }

    public void Clicked()
    {
        if (sceneInfo.Scenarios.Length == 0)
        {
            LoadScene();
        } else
        {
            VisualizerSelectionGridGenerator._Instance.UpdateSelection(sceneInfo.Scenarios, true, true);
        }
    }

    private void LoadScene()
    {
        VisualizerSelectionGridGenerator._Instance.LoadScene(sceneInfo);
    }

    public void SetAesthetics(float scenarioImagePadding, float textHolderVerticalPadding, float buttonFontSize)
    {
        RectTransformExtensions.SetAll(scenarioImage.transform as RectTransform, scenarioImagePadding);
        RectTransformExtensions.SetVerticals(textHolder.transform as RectTransform, textHolderVerticalPadding);
        scenarioNameText.fontSize = buttonFontSize;
    }
}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void SetAll(this RectTransform rt, float offset)
    {
        SetRight(rt, offset);
        SetLeft(rt, offset);
        SetTop(rt, offset);
        SetBottom(rt, offset);
    }

    public static void SetHorizontals(this RectTransform rt, float offset)
    {
        SetRight(rt, offset);
        SetLeft(rt, offset);
    }

    public static void SetVerticals(this RectTransform rt, float offset)
    {
        SetTop(rt, offset);
        SetBottom(rt, offset);
    }
}