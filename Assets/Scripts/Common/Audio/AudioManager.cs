using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] sourceArray;

    [SerializeField, Range(0, 1)] private float defaultVolume = 0.05f;
    [SerializeField, Range(0, 1)] private float defaultPitch = 1;

    private void Start()
    {
        FillSourceArray();
    }

    private void FillSourceArray()
    {
        sourceArray = GetComponents<AudioSource>();
    }

    // Here we loop through all of the audio sources on this game object and find the first one that is NOT playing/currently being used
    // From that audio source, we play our sound
    private void PlayFromIdleSource(AudioClip clip, float volume)
    {
        for (int i = 0; i < sourceArray.Length; i++)
        {
            if (IsSourceIdle(i))
            {
                PlayClipFromSourceArray(sourceArray[i], clip, volume);
            }
            break;
        }
    }

    // Determines whether or not the audio source at the speccified index is being played from or not
    private bool IsSourceIdle(int index)
    {
        if (sourceArray[index].isPlaying)
        {
            return false;
        }
        return true;
    }

    // Plays an AudioClip clip from an AudioSource source
    private void PlayClipFromSourceArray(AudioSource source, AudioClip clip, float volume)
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    // The method other scripts will call when they want to play a sound.
    // Imagine it as them requesting a ticket or something along those lines.
    public void PlaySound(AudioClip clip, float volume)
    {
        PlayFromIdleSource(clip, volume);
    }

    public void PlaySound(AudioClip clip)
    {
        PlayFromIdleSource(clip, defaultVolume);
    }

    public void PlayOneShot(AudioClip clip)
    {
        sourceArray[0].volume = defaultVolume;
        sourceArray[0].PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip, float pitch)
    {
        sourceArray[0].pitch = pitch;
        sourceArray[0].volume = defaultVolume;
        sourceArray[0].PlayOneShot(clip);
        sourceArray[0].pitch = defaultPitch;
    }

    public void PlayOneShot(AudioClip clip, float pitch, float vol)
    {
        sourceArray[0].pitch = pitch;
        sourceArray[0].volume = vol;
        sourceArray[0].PlayOneShot(clip);
        sourceArray[0].pitch = defaultPitch;
        sourceArray[0].volume = defaultVolume;
    }
}
