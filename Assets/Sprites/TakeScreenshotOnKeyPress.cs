using UnityEngine;

public class TakeScreenshotOnKeyPress : ActOnKeyPress
{
    [SerializeField] private string fileName;
    protected override void Act()
    {
        ScreenRenderTextureManager._Instance.TakeScreenshot(fileName);
    }
}
