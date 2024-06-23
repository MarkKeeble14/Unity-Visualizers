using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor.Media;
using Unity.Collections;
using UnityEngine.Recorder;
using UnityEditor.Recorder;
using UnityEditor;
using UnityEditor.Recorder.Input;

/// <summary>
/// Captures frames from a Unity camera in real time
/// and writes them to disk using a background thread.
/// </summary>
/// 
/// <description>
/// Maximises speed and quality by reading-back raw
/// texture data with no conversion and writing 
/// frames in uncompressed BMP format.
/// Created by Richard Copperwaite.
/// </description>
/// 
public class ScreenRecorder : MonoBehaviour
{

    private float maxRecordingTime = 5f;
    public float MaxRecordingTime { get { return maxRecordingTime; } set { maxRecordingTime = value; } }

    private float currentRecordingDuration;

    [Header("References")]
    [SerializeField] private bool showRecordingMarker = true;

    public static ScreenRecorder _Instance;

    [Header("Video Attributes")]
    [SerializeField] private int frameRate = 30; // number of frames to capture per second
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private string fileName;

    [Header("Audio Attributes")]
    [SerializeField] private int sampleRate = 48000;

    private RecorderControllerSettings recorderControllerSettings;
    private RecorderController recorderController;
    private MovieRecorderSettings videoRecorder;

    public string GetCurrentRecordingDuration(bool returnEmptyIfNotRecording)
    {
        if (!IsRecording)
            return (returnEmptyIfNotRecording ? "" : StringHelper.GetDurationText(0));
        return StringHelper.GetDurationText(currentRecordingDuration);
    }

    public bool IsRecording
    {
        get
        {
            if (recorderController == null) return false;
            return recorderController.IsRecording();
        }
    }

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);

        _Instance = this;

        recorderControllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(recorderControllerSettings);
        videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        videoRecorder.name = "VideoRecorder";
        RecorderOptions.VerboseMode = false;
    }

    [ContextMenu("StartRecording")]
    public void StartRecording()
    {
        currentRecordingDuration = 0;

        Debug.Log("Called to Begin Recording");
        videoRecorder.Enabled = true;
        videoRecorder.VideoBitRateMode = VideoBitrateMode.High;
        videoRecorder.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = width,
            OutputHeight = height
        };
        videoRecorder.AudioInputSettings.PreserveAudio = true;

        string savedFileName = string.Format(fileName + "_{0}", PlayerPrefs.GetInt("TakeNumber"));
        string encodedFilePath = Path.Combine(Application.dataPath, "../Recordings/", savedFileName + "/" + savedFileName);
        encodedFilePath = encodedFilePath.Replace("/", @"\");

        videoRecorder.OutputFile = encodedFilePath;

        recorderControllerSettings.AddRecorderSettings(videoRecorder);
        recorderControllerSettings.SetRecordModeToManual();
        recorderControllerSettings.FrameRate = frameRate;
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }

    [ContextMenu("StopRecording")]
    public void StopRecording()
    {
        Debug.Log("Called to End Recording");

        recorderController.StopRecording();

        PlayerPrefs.SetInt("TakeNumber", PlayerPrefs.GetInt("TakeNumber") + 1);
    }

    private void Update()
    {
        if (recorderController.IsRecording())
        {
            currentRecordingDuration += Time.deltaTime;

            if (currentRecordingDuration >= maxRecordingTime) { StopRecording(); }
        }
    }
}