using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeScreenshotOnKeyPress : ActOnKeyPress
{
    [SerializeField] private List<SerializableKeyValuePair<VisualizerElementLabel, bool>> defaultScreenieSettings = new();

    protected override void Act()
    {
        ScreenRenderTextureManager._Instance.TakeScreenshot(SceneManager.GetActiveScene().name);
    }
}
