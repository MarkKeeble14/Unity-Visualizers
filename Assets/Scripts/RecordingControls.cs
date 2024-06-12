using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingControls : MonoBehaviour
{
    [SerializeField] private KeyCode startRecording = KeyCode.X;
    [SerializeField] private KeyCode stopRecording = KeyCode.Z;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(startRecording))
        {
            RecordingManager._Instance.StartRecording();
        }
        else if (Input.GetKeyDown(stopRecording))
        {
            RecordingManager._Instance.StopRecording();
        }
    }
}
