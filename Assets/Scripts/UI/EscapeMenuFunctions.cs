using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenuFunctions : MonoBehaviour
{
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private Transform controlsDisplay;
    [SerializeField] private ControlDisplay controlDisplayPrefab;
    [SerializeField] private ControlScrollView listPrefab;
    [SerializeField] private ControlScheme defaultToControlScheme;
    [SerializeField] private TextMeshProUGUI activeSchemeText;
    [SerializeField] private GameObject prevSchemeButton;
    [SerializeField] private GameObject nextSchemeButton;
    [SerializeField] private string outTransition;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private int currentlyDisplayedControlSchemeIndex = 0;
    private List<SerializableKeyValuePair<ControlScheme, string>> availableControlSchemes = new();
    private Dictionary<ControlScheme, ControlScrollView> controlSchemeScrollViews = new();
    private ControlScrollView currentlyDisplayedControlScheme;
    private List<ControlScheme> foundControlSchemes = new();

    public static EscapeMenuFunctions _Instance { get; private set; }
    public bool IsOpen => cv.alpha == 1;

    public Action OnOpen;
    public Action OnClose;

    private bool musicMuted;
    private float savedMusicVolume;
    private bool sfxMuted;
    private float savedSFXVolume;

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }


    private void Start()
    {
        MakeControls();

        MakeResolutionDropdown();
    }

    private void Update()
    {
        prevSchemeButton.SetActive(currentlyDisplayedControlSchemeIndex > 0);
        nextSchemeButton.SetActive(currentlyDisplayedControlSchemeIndex < foundControlSchemes.Count - 1);
    }
    public void Open()
    {
        cv.alpha = 1;
        cv.blocksRaycasts = true;

        OnOpen?.Invoke();
    }

    public void Close()
    {
        cv.alpha = 0;
        cv.blocksRaycasts = false;

        SetOptionsMenuActive(false);

        OnClose?.Invoke();
    }

    public void Toggle()
    {
        if (cv.alpha == 0)
        {
            Open();
        }
        else
        {
            Close();
        }
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        TransitionManager._Instance.Transition(outTransition, TransitionDirection.IN, null, () =>
        {
            SceneManager.LoadScene(0);
        });
    }

    public void SetOptionsMenuActive(bool value)
    {
        optionsPanel.SetActive(value);
    }

    public void SwitchControlScheme()
    {
        List<ActionSelection> possibleActions = new List<ActionSelection>();
        ControlScheme activeScheme = VisualizerManager._Instance.ActiveControlScheme;
        foreach (SerializableKeyValuePair<ControlScheme, string> kvp in  availableControlSchemes)
        {
            if (kvp.Key == activeScheme) continue;
            possibleActions.Add(new ActionSelection(kvp.Value, () => VisualizerManager._Instance.SelectControlScheme(kvp.Key)));
        }
        Close();
        StartCoroutine(UIManager._Instance.PopupActionSelection("Select Control Scheme", "Cancel", null, possibleActions));
    }

    private bool IsSchemeAvailable(ControlScheme scheme)
    {
        foreach (SerializableKeyValuePair<ControlScheme, string> kvp in availableControlSchemes)
        {
            if (kvp.Key == scheme) return true;
        }
        return false;
    }

    private void MakeControls()
    {
        // Clear
        foreach (Transform child in controlsDisplay)
        {
            Destroy(child.gameObject);
        }

        KeyControl[] keyControls = FindObjectsOfType<KeyControl>(true);
        foreach (KeyControl control in keyControls)
        {
            if (control.Hidden) continue;

            if (!IsSchemeAvailable(control.PartOfScheme)) continue;

            Transform list;
            if (!controlSchemeScrollViews.ContainsKey(control.PartOfScheme))
            {
                ControlScrollView scrollView = Instantiate(listPrefab, controlsDisplay);
                list = scrollView.List;
                scrollView.gameObject.SetActive(false);
                foundControlSchemes.Add(control.PartOfScheme);
                controlSchemeScrollViews.Add(control.PartOfScheme, scrollView);
            } else
            {
                list = controlSchemeScrollViews[control.PartOfScheme].List;
            }

            ControlDisplay spawned = Instantiate(controlDisplayPrefab, list);
            spawned.Set(control.GetAction(), control.GetKey());
        }

        // set default
        currentlyDisplayedControlSchemeIndex = foundControlSchemes.IndexOf(defaultToControlScheme);

        UpdateDisplayedControlScheme();
    }

    public void UpdateDisplayedControls(ControlScheme controlScheme)
    {
        currentlyDisplayedControlSchemeIndex = foundControlSchemes.IndexOf(controlScheme);
        UpdateDisplayedControlScheme();
    }

    public void DisplayNextScheme()
    {
        if (currentlyDisplayedControlSchemeIndex >= foundControlSchemes.Count - 1) return;
        currentlyDisplayedControlSchemeIndex++;

        UpdateDisplayedControlScheme();
    }

    public void DisplayLastScheme()
    {
        if (currentlyDisplayedControlSchemeIndex <= 0) return;
        currentlyDisplayedControlSchemeIndex--;

        UpdateDisplayedControlScheme();
    }

    private void UpdateDisplayedControlScheme()
    {
        // deactivate old one
        if (currentlyDisplayedControlScheme != null)
        {
            currentlyDisplayedControlScheme.gameObject.SetActive(false);
        }

        // activate new one
        ControlScheme activeScheme = foundControlSchemes[currentlyDisplayedControlSchemeIndex];
        currentlyDisplayedControlScheme = controlSchemeScrollViews[activeScheme];
        currentlyDisplayedControlScheme.gameObject.SetActive(true);

        // set text
        foreach (SerializableKeyValuePair<ControlScheme, string> kvp in availableControlSchemes)
        {
            if (kvp.Key == activeScheme)
            {
                activeSchemeText.text = kvp.Value;
                break;
            }
        }

        if (activeScheme == VisualizerManager._Instance.ActiveControlScheme)
        {
            activeSchemeText.color = Color.green;
            activeSchemeText.text += " (Active)";
        } else
        {
            activeSchemeText.color = Color.red;
            activeSchemeText.text += " (Inactive)";
        }
    }

    public void SetAvailableControlSchemes(List<SerializableKeyValuePair<ControlScheme, string>> dict)
    {
        availableControlSchemes = dict;
    }

    public void SetMusicVolume(float volume)
    {
        savedMusicVolume = volume;
        if (!musicMuted)
            mixer.SetFloat("MusicVolume", ConvertPercentToDB(volume));
    }

    public void SetSFXVolume(float volume)
    {
        savedSFXVolume = volume;
        if (!sfxMuted)
            mixer.SetFloat("SFXVolume", ConvertPercentToDB(volume));
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetMusicMuted(bool value)
    {
        musicMuted = value;
        if (musicMuted)
        {
            mixer.SetFloat("MusicVolume", ConvertPercentToDB(0.0001f));
        } else
        {
            mixer.SetFloat("MusicVolume", ConvertPercentToDB(savedMusicVolume));
        }
    }

    public void SetSFXMuted(bool value)
    {
        sfxMuted = value;
        if (sfxMuted)
        {
            mixer.SetFloat("SFXVolume", ConvertPercentToDB(0.0001f));
        }
        else
        {
            mixer.SetFloat("SFXVolume", ConvertPercentToDB(savedSFXVolume));
        }
    }

    private float ConvertPercentToDB(float value)
    {
        return Mathf.Log10(value) * 20;
    }

    private void MakeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int curResolution = 0;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            options.Add(res.width + " x " + res.height);
            if (res.width == Screen.width && res.height == Screen.height)
            {
                curResolution = i;
                break;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(curResolution);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
