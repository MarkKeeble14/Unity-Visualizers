using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public abstract class PremadeVisualizer : SetElementColorToMatchTrack
{
    [Header("Segment Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Settings")]
    [SerializeField] protected AttachmentType attachmentType;
    [SerializeField] protected float signalMultiplier;
    [SerializeField] private float attachmentMultiplier;
    [SerializeField] private float defaultValue;
    [SerializeField] private float adjustSpeed;
    [SerializeField] protected bool useSlowAdjust = true;

    protected List<AttachParameter> attachedParameterList = new List<AttachParameter>();
    protected List<SignalBroadcaster> broadcasterList = new List<SignalBroadcaster>();
    protected List<Image> imageList = new List<Image>();

    private bool hasMadeVisualizer;

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
        if (attachmentType == AttachmentType.FIRST_256_SAMPLES || attachmentType == AttachmentType.AMPLITUDE_256)
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

                        TrackSegment(spawned, broadcaster, i, 8);
                    }
                    break;
            }
        } else // First N Samples Visualizer
        {
            for (int i = 0; i < n; i++)
            {
                GameObject spawned = Instantiate(segmentPrefab, transform);

                SignalBroadcaster broadcaster;

                if (attachmentType == AttachmentType.AMPLITUDE_256)
                {
                    broadcaster = spawned.AddComponent<AmplitudeBroadcaster>();
                } else
                {
                    broadcaster = spawned.AddComponent<SampleBroadcaster>();
                    ((SampleBroadcaster)broadcaster).Sample = i;
                }

                TrackSegment(spawned, broadcaster, i, n);
            }
        }
    }

    protected abstract void PreMakingVisualizer();

    protected virtual void UpdateSpecificSettings() { }

    [ContextMenu("UpdateAttachments")]
    protected void UpdateAttachments()
    {
        foreach (AttachParameter parameter in attachedParameterList)
        {
            parameter.Bypass = !Active;
            parameter.SlowAdjust = useSlowAdjust;
            parameter.DefaultValue = defaultValue;
            parameter.AdjustSpeed = adjustSpeed;
            parameter.Multiplier = attachmentMultiplier;
        }

        foreach (SignalBroadcaster broadcaster in broadcasterList)
        {
            broadcaster.UpdateSettings(signalMultiplier);
        }
    }

    protected new void Update()
    {
        base.Update();
        UpdateSpecificSettings();
    }

    private void Start()
    {
        PreMakingVisualizer();
    }

    protected void TrackSegment(GameObject segment, SignalBroadcaster broadcaster, int index, int max)
    {
        // Broadcaster
        broadcasterList.Add(broadcaster);

        // Attached Parameters
        attachedParameterList.Add(segment.GetComponent<AttachParameter>());

        // Image
        imageList.Add(segment.GetComponent<Image>());

        // Connect the broadcaster with it attachments
        segment.GetComponent<SignalDirector>().AddDirection(broadcaster, segment.GetComponents<AttachParameter>().ToList());
    }

    public override void RecieveTrackInfo(TrackInfo info)
    {
        base.RecieveTrackInfo(info);

        if (!hasMadeVisualizer)
        {
            MakeVisualizer();
            hasMadeVisualizer = true;
        }

        UpdateAttachments();
    }

    protected override void SetElementToColor(Color c)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            if (!Active)
                imageList[i].color = c;
            else
                imageList[i].color = VisualizerManager._Instance.GetColor(ColorType, ColorIndex, new Vector2(i, imageList.Count));
        }
    }
}