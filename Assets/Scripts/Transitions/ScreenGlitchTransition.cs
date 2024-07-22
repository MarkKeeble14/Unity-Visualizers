using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class ScreenGlitchTransition : Transition
{
    [SerializeField] private float inStartingDigitalIntensity;
    [SerializeField] private float inStartingAnalogColorDrift;
    [SerializeField] private float inStartingAnalogHorizontalShake;

    [SerializeField] private float intensityChangeRate;
    [SerializeField] private float colorDriftChangeRate;
    [SerializeField] private float horizontalShakeChangeRate;
    [SerializeField] private Volume volume;

    protected override IEnumerator TransitionIn(float speed = 1)
    {
        DigitalGlitchVolume digitalGlitchVolume;
        volume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);

        AnalogGlitchVolume analogGlitchVolume;
        volume.profile.TryGet<AnalogGlitchVolume>(out analogGlitchVolume);

        digitalGlitchVolume.intensity.Override(0);
        analogGlitchVolume.horizontalShake.Override(0);
        analogGlitchVolume.colorDrift.Override(0);

        bool needContinue = true;
        while (needContinue)
        {
            needContinue = false;
            if (digitalGlitchVolume.intensity.value < 1)
            {
                digitalGlitchVolume.intensity.Override(digitalGlitchVolume.intensity.value + (intensityChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            if (analogGlitchVolume.horizontalShake.value < 1)
            {
                analogGlitchVolume.horizontalShake.Override(analogGlitchVolume.horizontalShake.value + (horizontalShakeChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            if (analogGlitchVolume.colorDrift.value < 1)
            {
                analogGlitchVolume.colorDrift.Override(analogGlitchVolume.colorDrift.value + (colorDriftChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            yield return null;
        }

        digitalGlitchVolume.intensity.Override(1);
        analogGlitchVolume.horizontalShake.Override(1);
        analogGlitchVolume.colorDrift.Override(1);
    }

    protected override IEnumerator TransitionOut(float speed = 1)
    {
        DigitalGlitchVolume digitalGlitchVolume;
        volume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);

        AnalogGlitchVolume analogGlitchVolume;
        volume.profile.TryGet<AnalogGlitchVolume>(out analogGlitchVolume);

        digitalGlitchVolume.intensity.Override(inStartingDigitalIntensity);
        analogGlitchVolume.horizontalShake.Override(inStartingAnalogHorizontalShake);
        analogGlitchVolume.colorDrift.Override(inStartingAnalogColorDrift);

        bool needContinue = true;
        while (needContinue)
        {
            needContinue = false;
            if (digitalGlitchVolume.intensity.value > 0)
            {
                digitalGlitchVolume.intensity.Override(digitalGlitchVolume.intensity.value - (intensityChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            if (analogGlitchVolume.horizontalShake.value > 0)
            {
                analogGlitchVolume.horizontalShake.Override(analogGlitchVolume.horizontalShake.value - (horizontalShakeChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            if (analogGlitchVolume.colorDrift.value > 0)
            {
                analogGlitchVolume.colorDrift.Override(analogGlitchVolume.colorDrift.value - (colorDriftChangeRate * Time.deltaTime * speed));
                needContinue = true;
            }

            yield return null;
        }

        digitalGlitchVolume.intensity.Override(0);
        analogGlitchVolume.horizontalShake.Override(0);
        analogGlitchVolume.colorDrift.Override(0);
    }
}
