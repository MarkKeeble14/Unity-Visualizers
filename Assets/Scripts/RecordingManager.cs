using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class RecordingManager : MonoBehaviour
{
    private float maxRecordingTime = 5f;
    public float MaxRecordingTime { get { return maxRecordingTime; } set { maxRecordingTime = value; } }

    private float m_stopRecordingTimer = float.MaxValue;
    private VideoCapture videoCapture = null;

    [SerializeField] private string fileName;
    [SerializeField] private int takeNumber;

    [SerializeField] private bool showRecordingMarker = true;
    [SerializeField] private GameObject recordingMarker;

    public bool CurrentlyRecording 
    { 
        get
        {
            if (videoCapture == null) return false;
            return videoCapture.IsRecording;
        }
    }

    public static RecordingManager _Instance;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);

        _Instance = this;
    }

    public void StartRecording()
    {
        StartVideoCaptureTest();
    }

    public void StopRecording()
    {
        if (!CurrentlyRecording) return;
        videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
    }

    void Update()
    {
        if (showRecordingMarker) recordingMarker.SetActive(CurrentlyRecording);
        
        if (videoCapture == null || !videoCapture.IsRecording) return;
        
        if (Time.time > m_stopRecordingTimer) videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
    }

    void StartVideoCaptureTest()
    {
        Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log(cameraResolution);

        float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
        Debug.Log(cameraFramerate);

        VideoCapture.CreateAsync(false, delegate (VideoCapture videoCapture)
        {
            if (videoCapture != null)
            {
                this.videoCapture = videoCapture;
                Debug.Log("Created VideoCapture Instance!");

                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                this.videoCapture.StartVideoModeAsync(cameraParameters,
                    VideoCapture.AudioState.ApplicationAndMicAudio,
                    OnStartedVideoCaptureMode);
            }
            else
            {
                Debug.LogError("Failed to create VideoCapture Instance!");
            }
        });
    }

    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Video Capture Mode!");
        string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
        string fileName = string.Format(this.fileName +  "_{0}_{0}.mp4", takeNumber, timeStamp);
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        filePath = filePath.Replace("/", @"\");
        videoCapture.StartRecordingAsync(filePath, OnStartedRecordingVideo);
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Video Capture Mode!");
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
        m_stopRecordingTimer = Time.time + MaxRecordingTime;
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        takeNumber++;
    }
}
