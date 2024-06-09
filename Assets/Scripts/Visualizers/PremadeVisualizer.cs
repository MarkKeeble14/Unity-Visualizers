using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PremadeVisualizer : MonoBehaviour
{
    [Header("Segment Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Settings")]
    [SerializeField] protected AttachmentType attachmentType;
    [SerializeField] private VisualizerColorType colorType;
    [SerializeField] protected float multiplier = 25;
    [SerializeField] protected bool useSlowAdjust;

    protected List<AttachParameter> attachedParameterList = new List<AttachParameter>();
    protected List<SignalBroadcaster> broadcasterList = new List<SignalBroadcaster>();

    protected virtual void MakeVisualizer()
    {
        MakeDefaultVisualizer();
    }

    protected void MakeDefaultVisualizer()
    {
        int n = 0;
        if (attachmentType == AttachmentType.FIRST_64_SAMPLES)
            n = 64;
        if (attachmentType == AttachmentType.FIRST_128_SAMPLES)
            n = 128;
        if (attachmentType == AttachmentType.FIRST_256_SAMPLES)
            n = 256;
        if (attachmentType == AttachmentType.MAX_SAMPLES)
            n = 512;

        // Not a First N Samples Visualizer
        if (n == 0)
        {
            switch (attachmentType)
            {
                case AttachmentType.BAND:
                    for (int i = 0; i < 8; i++)
                    {
                        GameObject spawned = Instantiate(segmentPrefab, transform);

                        SignalBroadcaster broadcaster = spawned.AddComponent<BandBroadcaster>();
                        ((BandBroadcaster)broadcaster).Band = i;

                        TrackSegment(spawned);
                        SetColor(spawned.GetComponent<Image>(), i, 8);
                    }
                    break;
            }
        } else // First N Samples Visualizer
        {
            for (int i = 0; i < n; i++)
            {
                GameObject spawned = Instantiate(segmentPrefab, transform);

                SignalBroadcaster broadcaster = spawned.AddComponent<SampleBroadcaster>();
                ((SampleBroadcaster)broadcaster).Sample = i;

                TrackSegment(spawned);
                SetColor(spawned.GetComponent<Image>(), i, n);
            }
        }
    }

    protected abstract void PreMakingVisualizer();

    protected virtual void UpdateSpecificSettings() { }

    [ContextMenu("UpdateComponents")]
    protected void UpdateComponents()
    {
        foreach (AttachParameter parameter in attachedParameterList)
        {
            // htb.Set(multiplier, defaultHeight, adjustSpeed);
            parameter.SetSlowAdjust(useSlowAdjust);
        }
        foreach (SignalBroadcaster broadcaster in broadcasterList)
        {
            broadcaster.UpdateSettings(multiplier);
        }
    }

    protected virtual void Update()
    {
        UpdateSpecificSettings();
    }

    private void Start()
    {
        PreMakingVisualizer();
        MakeVisualizer();
        UpdateComponents();
    }

    protected void TrackSegment(GameObject segment)
    {
        // Broadcaster
        broadcasterList.Add(segment.GetComponent<SignalBroadcaster>());

        // Attached Parameters
        attachedParameterList.Add(segment.GetComponent<AttachParameter>());
    }

    protected void SetColor(Image image, int v, int max)
    {
        switch (colorType)
        {
            case VisualizerColorType.DOMINANT_COLOR:
                image.color = VisualizerManager._Instance.UserDefinedDominantColor;
                break;

            case VisualizerColorType.SECONDARY_COLOR:
                image.color = VisualizerManager._Instance.UserDefinedSecondaryColor;
                break;

            case VisualizerColorType.TERTIARY_COLOR:
                image.color = VisualizerManager._Instance.UserDefinedTertiaryColor;
                break;

            case VisualizerColorType.GRADIENT:
                image.color = VisualizerManager._Instance.UserDefinedGradient.Evaluate((float)v / max);
                break;
        }
    }
}