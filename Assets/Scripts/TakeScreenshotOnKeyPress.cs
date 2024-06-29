using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshotOnKeyPress : ActOnKeyPress
{
    [SerializeField] private string fileName;

    [SerializeField] private List<SerializableKeyValuePair<VisualizerElementLabel, bool>> screenieSettings = new();
    protected override void Act()
    {
        ScreenRenderTextureManager._Instance.TakeScreenshot(fileName);
    }
}
