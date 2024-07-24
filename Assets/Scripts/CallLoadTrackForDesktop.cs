using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallLoadTrackForDesktop : MonoBehaviour
{
    [SerializeField] private CycleAudioClips cycle;
    [SerializeField] private AudioSource source;

    public void LoadFile()
    {
        Cursor.visible = true;
        StartCoroutine(VisualizerManager._Instance.RunTrackSelection(clip => Cursor.visible = false));
    }
}
