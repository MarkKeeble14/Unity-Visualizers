using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingStateControlsImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private bool enableIfRecording;

    private void Update()
    {
        image.enabled = enableIfRecording == ScreenRecorder._Instance.IsRecording;
    }
}
