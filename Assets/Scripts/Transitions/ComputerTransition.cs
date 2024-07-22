using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class ComputerTransition : AnimatorTransition
{
    [Header("Settings")]
    [SerializeField] private float floatEquivalencyGrace = 0.1f;

    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Volume mainCameraVolume;
    [SerializeField] private UniversalAdditionalCameraData mainCameraData;
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private float fadeOutMainCameraVolumeRate = 0.1f;
    [SerializeField] private float mainCameraFadeOutGoal = 0.9f;

    [Header("In")]
    [SerializeField] private bool inDoGlitch;
    [SerializeField] private float inGlitchStart;
    [SerializeField] private float inGlitchChangeRate = 1;
    [SerializeField] private MathHelper.AlterationMethod inGlitchMethod;
    [SerializeField] private float inGlitchGoal;

    [Header("Out")]
    [SerializeField] private bool outDoGlitch;
    [SerializeField] private float outGlitchStart;
    [SerializeField] private float outGlitchChangeRate = 1;
    [SerializeField] private MathHelper.AlterationMethod outGlitchMethod;
    [SerializeField] private float outGlitchGoal;

    private DigitalGlitchVolume digitalGlitchVolume;

    private void Awake()
    {
        globalVolume.profile.TryGet<DigitalGlitchVolume>(out digitalGlitchVolume);
    }

    protected override void BeforeTransitionIn()
    {
        // 
        if (inDoGlitch)
        {
            StartCoroutine(AlterGlitchIn());
        }

        animator.enabled = true;
    }

    protected override void BeforeTransitionOut()
    {
        // 
        if (outDoGlitch)
        {
            StartCoroutine(AlterGlitchOut());
        }
        animator.enabled = true;
    }

    protected override void WhileTransitioningIn()
    {
        //
    }

    protected override void WhileTransitioningOut()
    {
        //
    }

    private IEnumerator FadeOutMainCameraVolume()
    {
        mainCameraTransform.localEulerAngles = new Vector3(mainCameraTransform.localEulerAngles.x, mainCameraTransform.localEulerAngles.y, 180);
        mainCameraData.renderPostProcessing = true;
        mainCameraVolume.weight = 1;
        while (mainCameraVolume.weight > mainCameraFadeOutGoal)
        {
            mainCameraVolume.weight -= Time.deltaTime * fadeOutMainCameraVolumeRate;
            yield return null;
        }
        mainCameraVolume.weight = 0;
        mainCameraData.renderPostProcessing = false;
        mainCameraTransform.localEulerAngles = new Vector3(mainCameraTransform.localEulerAngles.x, mainCameraTransform.localEulerAngles.y, 0);
    }

    private IEnumerator AlterGlitchIn()
    {
        StartCoroutine(FadeOutMainCameraVolume());

        digitalGlitchVolume.intensity.Override(inGlitchStart);
        while (Mathf.Abs(digitalGlitchVolume.intensity.value - inGlitchGoal) > floatEquivalencyGrace)
        {
            digitalGlitchVolume.intensity.Override(MathHelper.GetNextValue(digitalGlitchVolume.intensity.value, inGlitchGoal, inGlitchChangeRate,
                inGlitchMethod, true));

            yield return null;
        }
        digitalGlitchVolume.intensity.Override(inGlitchGoal);
    }

    private IEnumerator AlterGlitchOut()
    {
        StartCoroutine(FadeOutMainCameraVolume());

        digitalGlitchVolume.intensity.Override(outGlitchStart);
        while (Mathf.Abs(digitalGlitchVolume.intensity.value - outGlitchGoal) > floatEquivalencyGrace)
        {
            digitalGlitchVolume.intensity.Override(MathHelper.GetNextValue(digitalGlitchVolume.intensity.value, outGlitchGoal, outGlitchChangeRate,
                outGlitchMethod, true));

            yield return null;
        }
        digitalGlitchVolume.intensity.Override(outGlitchGoal);
    }

    protected override void TransitionInEnd()
    {
        // 
    }

    protected override void TransitionOutEnd()
    {
        //
    }

    protected override void TransitionEnd()
    {
        animator.enabled = false;
    }
}
