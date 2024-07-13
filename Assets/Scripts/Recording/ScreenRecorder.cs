using UnityEngine;
using System.IO;
using FFMpegCore;
using System.Threading.Tasks;

public class ScreenRecorder : MonoBehaviour
{
    public static ScreenRecorder _Instance;

    [Header("References")]
    [SerializeField] private CanvasGroup recordingUI;
    [SerializeField] private BlinkImage recordingMarker;

    [Header("Video Attributes")]
    [SerializeField] private int frameRate = 30; // number of frames to capture per second
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private string fileName;

    [Header("Audio Attributes")]
    [SerializeField] private int sampleRate = 48000;

    [Header("Other Settings")]
    [SerializeField] private bool showRecordingMarker = true;
    private float currentRecordingDuration;
    private bool isRecording;
    public bool IsRecording => isRecording;

    private string recordingFolderPath;
    private string savedFramesPath;
    private string curFramePath;
    private int savedFrames;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);

        _Instance = this;

        // Create folders if neccessary
        recordingFolderPath = Path.Combine(Application.dataPath, "../Recordings");
        if (!Directory.Exists(recordingFolderPath))
        {
            Directory.CreateDirectory(recordingFolderPath);
        }
        savedFramesPath = Path.Combine(recordingFolderPath, "frames");

        // should probably put this somewhere else
        ConfigureFFMpeg();
    }

    private void Update()
    {
        if (isRecording)
        {
            currentRecordingDuration += Time.deltaTime;
        }
    }

    [ContextMenu("StartRecording")]
    public void StartRecording()
    {
        ScreenRenderTextureManager._Instance.RequestRenderToTex();

        currentRecordingDuration = 0;
        isRecording = true;
        recordingMarker.ResetTimer();

        // create saved frames directory
        if (!Directory.Exists(savedFramesPath))
        {
            Directory.CreateDirectory(savedFramesPath);
        }

        ScreenRenderTextureManager._Instance.AddToOnUpdateRenderTexture(RecordFrame);
    }

    private void RecordFrame(Texture2D frame)
    {
        // figure out file name
        curFramePath = savedFramesPath + "/" + savedFrames + ".png";
        ++savedFrames;

        // Write to file
        File.WriteAllBytes(curFramePath, frame.EncodeToPNG());
    }

    private string[] GetFrameNames()
    {
        string[] names = new string[savedFrames];
        for (int i = 0; i < savedFrames; i++)
        {
            names[i] = savedFramesPath + "/" + i + ".png";
        }
        return names;
    }

    [ContextMenu("StopRecording")]
    public async void StopRecording()
    {
        UnityEngine.Debug.Log("Called to End Recording");

        ScreenRenderTextureManager._Instance.RescindRenderToTexRequest();

        ScreenRenderTextureManager._Instance.RemoveFromOnUpdateRenderTexture(RecordFrame);

        isRecording = false;

        string outputPath = Path.Combine(recordingFolderPath, fileName + PlayerPrefs.GetInt("TakeNumber") + ".mp4");
        PlayerPrefs.SetInt("TakeNumber", PlayerPrefs.GetInt("TakeNumber") + 1);

        int loadingKey = UIManager._Instance.AddLoading("Saving video to file...");

        var task = Task.Run(async () => await JoinFramesIntoVideo(outputPath));
        await task;

        UIManager._Instance.RemoveLoading(loadingKey);

        UIManager._Instance.AddNewMessage("Successfully saved video to path: " + outputPath);
    }

    private void ConfigureFFMpeg()
    {
        GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Application.dataPath + "/StreamingAssets/FFMpeg/bin" });
    }

    private ValueTask<bool> JoinFramesIntoVideo(string outputPath)
    {
        FFMpeg.JoinImageSequence(outputPath, frameRate, GetFrameNames());

        // delete files and directory
        DirectoryInfo savedFramesDirectory = new DirectoryInfo(savedFramesPath);
        foreach (FileInfo file in savedFramesDirectory.EnumerateFiles())
        {
            file.Delete();
        }

        return new ValueTask<bool>(true);
    }


    public void OpenRecordingUI()
    {
        recordingUI.alpha = 1;
        recordingUI.blocksRaycasts = true;
    }

    public void CloseRecordingUI()
    {
        recordingUI.alpha = 0;
        recordingUI.blocksRaycasts = false;

        if (isRecording)
        {
            StopRecording();
        }
    }

    public string GetCurrentRecordingDuration(bool returnEmptyIfNotRecording)
    {
        if (!isRecording)
            return returnEmptyIfNotRecording ? "" : StringHelper.GetDurationText(0);
        return StringHelper.GetDurationText(currentRecordingDuration);
    }
}
