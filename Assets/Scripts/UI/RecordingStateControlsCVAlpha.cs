using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingStateControlsCVAlpha : MonoBehaviour
{
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private bool enableIfRecording;

    private void Update()
    {
        cv.alpha = enableIfRecording == ScreenRecorder._Instance.IsRecording ? 1 : 0;
    }
}
