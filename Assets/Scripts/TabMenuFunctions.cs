using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuFunctions : MonoBehaviour
{
    [SerializeField] private RectTransform tabList;
    [SerializeField] private float hiddenX;
    [SerializeField] private float shownX;
    [SerializeField] private float moveSpeed;
    [SerializeField] private MathHelper.AlterationMethod moveMethod;
    private bool showMenu;

    [SerializeField] private bool allowOpenTabMenuWithEscapeOpen;
    [SerializeField] private CanvasGroup escapeMenuCV;

    public void SetShowMenu()
    {
        if (!allowOpenTabMenuWithEscapeOpen && escapeMenuCV.alpha == 1) return;
        showMenu = true;
    }

    private void Update()
    {
        if (showMenu)
        {
            if (tabList.anchoredPosition.x < shownX)
            {
                tabList.anchoredPosition = new Vector2(MathHelper.GetNextValue(tabList.anchoredPosition.x, shownX, moveSpeed, moveMethod, true), tabList.anchoredPosition.y);
            }
        } else
        {
            if (tabList.anchoredPosition.x > hiddenX)
            {
                tabList.anchoredPosition = new Vector2(MathHelper.GetNextValue(tabList.anchoredPosition.x, hiddenX, moveSpeed, moveMethod, true), tabList.anchoredPosition.y);
            }
        }
        showMenu = false;
    }

    public void LoadTrack()
    {
        StartCoroutine(VisualizerManager._Instance.RunTrackSelection());
    }

    public void LoadCoverArt()
    {
        StartCoroutine(VisualizerManager._Instance.RunCoverArtSelection());
    }

    public void EditTitle()
    {
        VisualizerManager._Instance.EditTrackTitle();
    }

    public void OpenColorSettings()
    {
        StartCoroutine(VisualizerManager._Instance.RunColorsSelection());
    }

    public void OpenFontSettings()
    {
        StartCoroutine(VisualizerManager._Instance.RunFontsSelection());
    }

    public void OpenBaseVisualizerSettings()
    {
        StartCoroutine(VisualizerManager._Instance.RunEditBaseVisualizerElements());
    }

    public void OpenVisualizerSpecificSettings()
    {
        StartCoroutine(VisualizerManager._Instance.RunEditVisualizerSpecificElements());
    }

    public void OpenGeneralSettings()
    {
        StartCoroutine(VisualizerManager._Instance.RunEditGeneralSettings());
    }


    public void OpenPresetOptions()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Preset Options", "Cancel", null, new List<ActionSelection>()
        {
            new ActionSelection("Save Preset", () => VisualizerManager._Instance.SavePreset()),
            new ActionSelection("Load Preset", () => VisualizerManager._Instance.LoadPresetFromFile()),
        }));
    }
}
