using System.Collections.Generic;
using UnityEngine;

public class SFXManager :MonoBehaviour
{
    public static SFXManager _Instance { get; private set; }

    [SerializeField] private AudioSource source;
    [SerializeField] private List<SerializableKeyValuePair<string, OneShotContainer>> oneShotDict = new();
    [SerializeField] private List<string> disabledKeys = new();

    [SerializeField] private List<SerializableKeyValuePair<string, AudioSourceContainer>> audioSourceDict = new();

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    private string FitKey(string key) => key.ToUpper();

    public OneShotContainer PlayOneShot(string key, bool randomizeVolume, bool randomizePitch)
    {
        // fit key
        key = FitKey(key);

        if (disabledKeys.Contains(key)) return null;

        // try to play one shot
        OneShotContainer oneShotContainer = GetOneShotContainer(FitKey(key));
        if (oneShotContainer != null) { oneShotContainer.PlayOneShot(source, randomizeVolume, randomizePitch); }
        return oneShotContainer;
    }

    private OneShotContainer GetOneShotContainer(string key)
    {
        foreach (SerializableKeyValuePair<string, OneShotContainer> kvp in oneShotDict)
        {
            if (kvp.Key.Equals(key)) return kvp.Value;
        }
        return null;
    }

    public AudioSourceContainer PlaySource(string key, bool randomizeVolume, bool randomizePitch)
    {
        // fit key
        key = FitKey(key);

        if (disabledKeys.Contains(key)) return null;

        // try to play one shot
        AudioSourceContainer audioSourceContainer = GetAudioSourceContainer(FitKey(key));
        if (audioSourceContainer != null) { audioSourceContainer.PlaySource(randomizeVolume, randomizePitch); }
        return audioSourceContainer;
    }

    public void PlaySourceWithIncreasingVolume(string key, float startVol, float goalVol, float rateOfChange, MathHelper.AlterationMethod method)
    {
        AudioSourceContainer source = GetAudioSourceContainer(key);
        source.SetVolume(startVol);
        source.PlaySource(false, false);
        StartCoroutine(source.ChangeVolume(goalVol, rateOfChange, method));
    }

    private AudioSourceContainer GetAudioSourceContainer(string key)
    {
        foreach (SerializableKeyValuePair<string, AudioSourceContainer> kvp in audioSourceDict)
        {
            if (kvp.Key.Equals(key)) return kvp.Value;
        }
        return null;
    }

    public void PlayOneShotWithRandomSettings(string key) => PlayOneShot(key, true, true);
}
