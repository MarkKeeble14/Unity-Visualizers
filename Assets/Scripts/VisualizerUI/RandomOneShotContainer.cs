using System.Collections.Generic;
using UnityEngine;

public class RandomOneShotContainer : OneShotContainer
{
    [SerializeField] private List<AudioClip> clips;

    protected override void PlayOneShot(AudioSource source)
    {
        source.PlayOneShot(RandomHelper.GetRandomFromList(clips));
    }
}
