using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.DigitalGlitch;

public class LinkAudioSourceVolumeToDigitalGlitchIntensity : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private Volume volume;
    private DigitalGlitchVolume dGlitchVolume;

    [SerializeField] private float minValueVolume = 0;
    [SerializeField] private float maxValueVolume = 1;

    private void Awake()
    {
        volume.profile.TryGet<DigitalGlitchVolume>(out dGlitchVolume);
    }

    // Update is called once per frame
    void Update()
    {
        source.volume = MathHelper.Normalize(dGlitchVolume.intensity.value, 0, 1, minValueVolume, maxValueVolume);
    }
}
