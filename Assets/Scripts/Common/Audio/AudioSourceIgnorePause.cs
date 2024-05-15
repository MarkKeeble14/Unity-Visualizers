using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceIgnorePause : MonoBehaviour
{
    private void Awake()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.ignoreListenerPause = true;
    }
}
