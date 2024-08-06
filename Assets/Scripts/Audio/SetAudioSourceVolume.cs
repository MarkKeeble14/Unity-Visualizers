using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioSourceVolume : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
