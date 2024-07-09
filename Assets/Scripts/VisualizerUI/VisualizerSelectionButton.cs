using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VisualizerSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scenarioNameText;
    [SerializeField] private Image scenarioImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Button button;
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
            VisualizerSelectionGridGenerator._Instance.UpdateSelection(sceneInfo.Scenarios, true);
        }
    }

    private void LoadScene()
    {
        TransitionManager._Instance.Transition(sceneInfo.TransitionName, TransitionDirection.IN,
            () => TransitionManager._Instance.PlayAudioClip(sceneInfo.OnSelectSound), 
            () => SceneManager.LoadScene(sceneInfo.LoadSceneName));
    }
}
