using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleAudioClips : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    private int currentId = 0;
    private bool hasStarted;

    [ContextMenu("Skip")]
    private void Skip()
    {
        currentId++;
        if (currentId > clips.Count - 1) currentId = 0;
        UpdateClip();
    }

    [ContextMenu("Next")]
    private void Back()
    {
        currentId--;
        if (currentId < 0) { currentId = clips.Count - 1; }
        UpdateClip();
    }

    private void UpdateClip()
    {
        source.clip = clips[currentId];
        source.Play();
    }

    private void Awake()
    {
        source.clip = clips[currentId];
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted && source.isPlaying)
        {
            hasStarted = true;
        }

        if (hasStarted)
        {
            if (!source.isPlaying)
            {
                hasStarted = false;
                Skip();
            }
        }
    }
}
