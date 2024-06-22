using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRecorderCaller : MonoBehaviour
{
    public void StartRecording()
    {
        ScreenRecorder._Instance.StartRecording();
    }

    public void StopRecording()
    {
        ScreenRecorder._Instance.StopRecording();
    }
}
