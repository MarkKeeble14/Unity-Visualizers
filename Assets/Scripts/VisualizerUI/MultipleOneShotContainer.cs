using System.Collections.Generic;
using UnityEngine;

public class MultipleOneShotContainer : OneShotContainer
{
    [SerializeField] private List<AudioClip> clips;

    protected override void PlayOneShot(AudioSource source)
    {
        foreach (AudioClip clip in clips)
        {
            source.PlayOneShot(clip);
        }
    }
}
