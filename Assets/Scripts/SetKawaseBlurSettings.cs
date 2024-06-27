using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetKawaseBlurSettings : MonoBehaviour
{
    [Header("Blur Settings")]
    [SerializeField] private KawaseBlurSettings blurSettings;
    private KawaseBlur kawaseBlurPass;

    [SerializeField] private UniversalRendererData urpData;

    private void Start()
    {
        // Fetch Kawase Blur Feature
        kawaseBlurPass = (KawaseBlur)urpData.rendererFeatures[0];

        // Change Settings
        SetKawaseBlurFeatureSettings();
    }

    private void SetKawaseBlurFeatureSettings()
    {
        kawaseBlurPass.SetActive(blurSettings.Enabled);
        kawaseBlurPass.settings.blurPasses = blurSettings.BlurPasses;
        kawaseBlurPass.settings.downsample = blurSettings.Downsample;
        kawaseBlurPass.settings.copyToFramebuffer = blurSettings.CopyToFrameBuffer;
    }
}
