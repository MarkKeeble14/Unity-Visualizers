using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public struct ScreenshotSettings
{
    public string FileName;
    public Dictionary<VisualizerElementLabel, VisualizerElementsSettings> VisualizerElementStates;

    public ScreenshotSettings(string fileName, Dictionary<VisualizerElementLabel, VisualizerElementsSettings> visualizerElementStates)
    {
        FileName = fileName;
        VisualizerElementStates = visualizerElementStates;
    }
}

public class ScreenRenderTextureManager : MonoBehaviour
{
    public static ScreenRenderTextureManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;

        renderTex.width = Screen.width;
        renderTex.height = Screen.height;
    }

    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private RenderTexture renderTex;

    public Texture2D Tex { get; private set; }

    private Action OnPreUpdateRenderTexture;
    private Action<Texture2D> OnUpdateRenderTexture;

    private int numRenderRequests;

    private bool screenshotDecisionMade;
    private bool screenshotCancelled;

    [Header("References")]
    [SerializeField] private GameObject screenshotCanvas;
    [SerializeField] private List<SerializableKeyValuePair<VisualizerElementLabel, Checkbox>> enableVisualizerElementCheckboxes = new();

    public void RequestRenderToTex()
    {
        numRenderRequests++;
        StartCoroutine(RenderTexLoop());
    }

    public void RescindRenderToTexRequest()
    {
        numRenderRequests--;
    }

    public void CancelScreenshot()
    {
        screenshotDecisionMade = true;
        screenshotCancelled = true;
    }

    public void ConfirmScreenshot()
    {
        screenshotDecisionMade = true;
        screenshotCancelled = false;
    }

    private IEnumerator RenderTexLoop()
    {
        while (numRenderRequests > 0)
        {
            yield return new WaitForEndOfFrame();

            UpdateScreenTexture();
        }
    }

    private void UpdateScreenTexture()
    {
        OnPreUpdateRenderTexture?.Invoke();

        Tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        Tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        Tex.Apply();

        // Invoke callback
        OnUpdateRenderTexture?.Invoke(Tex);
    }


    private IEnumerator Screenshot(string fileName)
    {
        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0;
        VisualizerManager._Instance.PausePlayback();

        screenshotCanvas.SetActive(true);

        yield return new WaitUntil(() => screenshotDecisionMade);
        screenshotDecisionMade = false;

        screenshotCanvas.SetActive(false);

        if (!screenshotCancelled)
        {
            // Make new values
            Dictionary<VisualizerElementLabel, VisualizerElementsSettings> newVisualElementSettings = MakeScreenieSettings();

            // Store previous values
            Dictionary<VisualizerElementLabel, VisualizerElementsSettings> previousVisualElementSettings = VisualizerManager._Instance.GetVisualizerElementSettings();

            RequestRenderToTex();

            VisualizerManager._Instance.SetVisualizerElementsSettings(newVisualElementSettings);

            yield return new WaitForEndOfFrame();

            SaveScreenshot(fileName);

            RescindRenderToTexRequest();

            yield return new WaitForEndOfFrame();

            VisualizerManager._Instance.SetVisualizerElementsSettings(previousVisualElementSettings);
        }

        Time.timeScale = prevTimeScale;
        VisualizerManager._Instance.ResumePlayback();
    }

    public void TakeScreenshot(string fileName)
    {
        StartCoroutine(Screenshot(fileName));
    }

    private Dictionary<VisualizerElementLabel, VisualizerElementsSettings> MakeScreenieSettings()
    {
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> currentDict = VisualizerManager._Instance.GetVisualizerElementSettings();
        Dictionary<VisualizerElementLabel, VisualizerElementsSettings> result = new();
        foreach (SerializableKeyValuePair<VisualizerElementLabel, Checkbox> kvp in enableVisualizerElementCheckboxes)
        {
            VisualizerElementsSettings cur = currentDict[kvp.Key];
            result.Add(kvp.Key, new VisualizerElementsSettings(cur.ColorType, cur.ColorIndex, cur.FontIndex, kvp.Value.Active));
        }
        return result;
    }

    private void SaveScreenshot(string fileName)
    {
        // Create folder if neccessary
        string folderPath = Path.Combine(Application.dataPath, "../Screenshots");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Figure out path/file name
        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        string filePath = Path.Combine(folderPath, fileName + "_" + dirInfo.GetFiles().Length + ".png");
        filePath = filePath.Replace("/", @"\");

        // Write to file
        File.WriteAllBytes(filePath, Tex.EncodeToPNG());
    }
}
