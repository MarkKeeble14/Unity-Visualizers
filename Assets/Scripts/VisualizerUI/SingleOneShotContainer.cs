using UnityEngine;

public class SingleOneShotContainer : OneShotContainer
{
    [SerializeField] private AudioClip clip;

    protected override void PlayOneShot(AudioSource source)
    {
        source.PlayOneShot(clip);
    }
}
