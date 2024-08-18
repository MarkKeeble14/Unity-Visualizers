using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallLoadTrackForDesktop : MonoBehaviour
{
    [SerializeField] private CycleAudioClips cycle;
    [SerializeField] private AudioSource source;

    public void LoadFile()
    {
        StartCoroutine(VisualizerManager._Instance.RunTrackSelection(null));
    }
}
