using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuFunctions : MonoBehaviour
{
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private List<SerializableKeyValuePair<ControlScheme, string>> availableControlSchemes = new();
    [SerializeField] private Transform controlsDisplay;
    [SerializeField] private ControlDisplay controlDisplayPrefab;
    [SerializeField] private ControlScrollView listPrefab;
    [SerializeField] private ControlScheme defaultToControlScheme;
    private int currentlyDisplayedControlSchemeIndex = 0;
    private Dictionary<ControlScheme, ControlScrollView> controlSchemeScrollViews = new();
    private ControlScrollView currentlyDisplayedControlScheme;
    private List<ControlScheme> foundControlSchemes = new();
    [SerializeField] private TextMeshProUGUI activeSchemeText;
    [SerializeField] private GameObject prevSchemeButton;
    [SerializeField] private GameObject nextSchemeButton;

    private void Start()
    {
        MakeControls();
    }

    private void Update()
    {
        prevSchemeButton.SetActive(currentlyDisplayedControlSchemeIndex > 0);
        nextSchemeButton.SetActive(currentlyDisplayedControlSchemeIndex < foundControlSchemes.Count - 1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        cv.blocksRaycasts = false;
        cv.alpha = 0;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenVisualizerSetup()
    {
        Resume();
        StartCoroutine(VisualizerManager._Instance.RunVisualizerElementsSelection());
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
        Resume();
        StartCoroutine(UIManager._Instance.PopupActionSelection("Select Control Scheme", "Cancel", null, possibleActions));
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
}
