using System.Collections;
using UnityEngine;

public class AudioSourceContainer : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private Vector2 minMaxVolume;
    [SerializeField] private Vector2 minMaxPitch;

    public void PlaySource(bool randomizeVolume, bool randomizePitch)
    {
        if (randomizeVolume)
        {
            source.volume = RandomHelper.RandomFloat(minMaxVolume);
        }

        if (randomizePitch)
        {
            source.pitch = RandomHelper.RandomFloat(minMaxPitch);
        }

        source.Play();
    }

    public void SetVolume(float volume) { source.volume = volume; }

    public void SetPitch(float pitch) { source.pitch = pitch; }

    public IEnumerator ChangeVolume(float goal, float rateOfChange, MathHelper.AlterationMethod method)
    {
        while (source.volume != goal)
        {
            source.volume = MathHelper.GetNextValue(source.volume, goal, rateOfChange, method, true);

            yield return null;
        }
    }
}
