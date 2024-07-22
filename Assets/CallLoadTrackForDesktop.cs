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
        StartCoroutine(VisualizerManager._Instance.BrowseForTrackFile(clip =>
        {
            cycle.Pause = true;
            source.Stop();
            source.clip = clip;
            source.Play();
            cycle.Pause = false;
        }, () => Cursor.visible = false));
    }
}
