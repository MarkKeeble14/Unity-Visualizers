using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

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
    private Queue<string> screenieQueue = new();
    public Texture2D Tex { get; private set; }

    private Action<Texture2D> OnUpdateRenderTexture;

    private int numRenderRequests;

    public void RequestRenderToTex()
    {
        numRenderRequests++;
        StartCoroutine(RenderTexLoop());
    }

    public void RescindRenderToTexRequest()
    {
        numRenderRequests--;
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
        Tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        Tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        Tex.Apply();

        // Invoke callback
        OnUpdateRenderTexture?.Invoke(Tex);
    }

    public void TakeScreenshot(string fileName)
    {
        // Queue up call back
        OnUpdateRenderTexture += Screenshot;
        
        // Add fileName to queue
        screenieQueue.Enqueue(fileName);

        RequestRenderToTex();
    }

    private void Screenshot(Texture2D tex)
    {
        // Create folder if neccessary
        string folderPath = Path.Combine(Application.dataPath, "../Screenshots");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Figure out path/file name
        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
        string filePath = Path.Combine(folderPath, screenieQueue.Dequeue() + "_" + dirInfo.GetFiles().Length + ".png");
        filePath = filePath.Replace("/", @"\");

        // Write to file
        File.WriteAllBytes(filePath, Tex.EncodeToPNG());

        RescindRenderToTexRequest();

        // Remove callback
        OnUpdateRenderTexture -= Screenshot;
    }
}
