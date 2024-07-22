using UnityEngine;

public abstract class OneShotContainer : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxVolume = new Vector2(1, 1);
    [SerializeField] private Vector2 minMaxPitch = new Vector2(1, 1);

    protected abstract void PlayOneShot(AudioSource source);

    public void PlayOneShot(AudioSource source, bool randomizeVolume, bool randomizePitch)
    {
        if (randomizeVolume)
        {
            source.volume = RandomHelper.RandomFloat(minMaxVolume);
        }

        if (randomizePitch)
        {
            source.pitch = RandomHelper.RandomFloat(minMaxPitch);
        }

        PlayOneShot(source);
    }
}
