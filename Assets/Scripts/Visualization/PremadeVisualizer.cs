using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class PremadeVisualizer : SetVisualizerElementColor
{
    [Header("Segment Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Settings")]
    [SerializeField] protected AttachmentType attachmentType;
    public AttachmentType AttachmentType 
    { 
        get 
        { 
            return attachmentType; 
        } 
        set 
        { 
            attachmentType = value; 
            Reconstruct(); 
        } 
    }

    [SerializeField] protected float signalMultiplier;
    [SerializeField] private float defaultValue;
    [SerializeField] private float adjustSpeed;
    [SerializeField] protected bool useSlowAdjust = true;

    protected List<AttachParameter> attachedParameterList = new List<AttachParameter>();
    protected List<SignalBroadcaster> broadcasterList = new List<SignalBroadcaster>();
    protected List<Image> imageList = new List<Image>();
    protected List<RectTransform> segmentList = new List<RectTransform>();

    public void SetFloatSettings(float signalMultiplier, float defaultValue, float adjustSpeed)
    {
        this.signalMultiplier = signalMultiplier;
        this.defaultValue = defaultValue;
        this.adjustSpeed = adjustSpeed;
        UpdateAttachments();
    }

    private void Start()
    {
        MakeDefaultVisualizer();
    }

    protected new void Update()
    {
        base.Update();

        UpdateSpecificSettings();
    }

    protected abstract void PreMakingVisualizer();

    protected abstract void PostMakingVisualizer();

    protected virtual void UpdateSpecificSettings() { }


    [ContextMenu("Reconstruct")]
    public void Reconstruct()
    {
        foreach (Transform child in segmentList)
        {
            Destroy(child.gameObject);
        }

        attachedParameterList.Clear();
        broadcasterList.Clear();
        imageList.Clear();
        segmentList.Clear();

        MakeDefaultVisualizer();
    }

    protected void MakeDefaultVisualizer()
    {
        PreMakingVisualizer();

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

        UpdateAttachments();

        PostMakingVisualizer();
    }

    [ContextMenu("UpdateAttachments")]
    protected void UpdateAttachments()
    {
        foreach (AttachParameter parameter in attachedParameterList)
        {
            parameter.Bypass = !Active;
            parameter.SlowAdjust = useSlowAdjust;
            parameter.DefaultValue = defaultValue;
            parameter.AdjustSpeed = adjustSpeed;
        }

        foreach (SignalBroadcaster broadcaster in broadcasterList)
        {
            broadcaster.UpdateSettings(signalMultiplier);
        }
    }

    protected void TrackSegment(GameObject segment, SignalBroadcaster broadcaster, int index, int max)
    {
        // Broadcaster
        broadcasterList.Add(broadcaster);

        // Attached Parameters
        attachedParameterList.Add(segment.GetComponent<AttachParameter>());

        // Image
        imageList.Add(segment.GetComponentInChildren<Image>());

        segmentList.Add(segment.GetComponent<RectTransform>());

        // Connect the broadcaster with it attachments
        segment.GetComponent<SignalDirector>().AddDirection(broadcaster, segment.GetComponents<AttachParameter>().ToList());
    }

    protected override void SetElementToColor(Color c)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            if (!Active)
                imageList[i].color = c;
            else
                imageList[i].color = VisualizerManager._Instance.GetColor(ColorType, ColorIndex, (float)i / imageList.Count);
        }
    }
}