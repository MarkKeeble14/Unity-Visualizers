using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class ComputerBlinkTransition : Transition
{
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxNumBlinks;
    [SerializeField] private Vector2 minMaxTimeBetweenBlinks;
    [SerializeField] private Vector2 minMaxRestTime;
    [SerializeField] private Vector2 minMaxDigitalGlitchIntensityPerBlink;
    [SerializeField] private Vector2 minMaxIncreaseBlinkSpeedPerBlink;
    [SerializeField] private float colorDrift = 1;
    [SerializeField] private float horizontalShake = 1;
    [SerializeField] private float minBlinkSpeed = 0.25f;

    [Header("References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI desktopText;
    [SerializeField] private GameObject[] disableOnBlink;
    [SerializeField] private GameObject[] enableOnBlink;
    [SerializeField] private AudioSource source;
    [SerializeField] private CycleAudioClips cycle;
    [SerializeField] private UniversalAdditionalCameraData otherCamera;
    [SerializeField] private Volume volume;

    protected override IEnumerator TransitionIn()
    {
        float blinkSpeed = 1;

        DigitalGlitchVolume digitalGlitchVolume;
        volume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);

        AnalogGlitchVolume analogGlitchVolume;
        volume.profile.TryGet<AnalogGlitchVolume>(out analogGlitchVolume);

        VisualizerSelectionInfo selected = VisualizerSelectionGridGenerator._Instance.SelectedScene;
        backgroundImage.sprite = selected.ScenarioBackgroundSprite;
        desktopText.text = selected.ScenarioName;
        desktopText.font = selected.TextFont;
        desktopText.color = selected.TextColor;

        cycle.enabled = false;
        source.Stop();

        int numBlinks = RandomHelper.RandomIntInclusive(minMaxNumBlinks);
        for (int i = 0; i < numBlinks; i++)
        {
            blinkSpeed -= RandomHelper.RandomFloat(minMaxIncreaseBlinkSpeedPerBlink);
            blinkSpeed = Mathf.Clamp(blinkSpeed, minBlinkSpeed, 1);
            foreach (GameObject go in enableOnBlink)
            {
                go.SetActive(true);
            }

            // increase digital glitch intensity
            digitalGlitchVolume.intensity.Override(digitalGlitchVolume.intensity.value + RandomHelper.RandomFloat(minMaxDigitalGlitchIntensityPerBlink));

            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenBlinks) * blinkSpeed);

            foreach (GameObject go in disableOnBlink)
            {
                go.SetActive(false);
            }

            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenBlinks) * blinkSpeed);
        }

        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenBlinks));

        foreach (GameObject go in enableOnBlink)
        {
            go.SetActive(true);
        }

        // set analog glitch effects
        analogGlitchVolume.colorDrift.Override(colorDrift);
        analogGlitchVolume.horizontalShake.Override(horizontalShake);

        otherCamera.renderPostProcessing = true;

        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxRestTime));
    }

    protected override IEnumerator TransitionOut()
    {
        yield return StartCoroutine(TransitionIn());
    }
}
