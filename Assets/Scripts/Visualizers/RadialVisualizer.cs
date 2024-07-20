using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RadialLayoutGroup))]
public class RadialVisualizer : PremadeVisualizer
{
    [Header("Settings")]
    [SerializeField] private float distance = 0;
    [SerializeField] private float segmentWidth = 10;
    private float lastSegmentWidth;

    [Header("Adjustable Settings")]
    [SerializeField] private string distanceKey;
    [SerializeField] private string segmentWidthKey;

    private RadialLayoutGroup layoutGroup;

    protected override void UpdateSpecificSettings()
    {
        if (layoutGroup == null)
            layoutGroup = GetComponent<RadialLayoutGroup>();

        // radial distance
        layoutGroup.UpdateDistance(distance);

        // segment width
        if (lastSegmentWidth != segmentWidth)
        {
            Vector2 newDelta = new Vector2(segmentWidth, 0);
            foreach (RectTransform rect in segmentList)
            {
                rect.sizeDelta = newDelta;
            }
        }
        lastSegmentWidth = segmentWidth;
    }

    public override void RecieveVisualizerFloatValues(Dictionary<string, float> settings)
    {
        if (ignoreBroadcasts) return;

        base.RecieveVisualizerFloatValues(settings);

        if (!settings.ContainsKey(distanceKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(distanceKey, distance);
        }
        else
        {
            distance = settings[distanceKey];
        }

        if (!settings.ContainsKey(segmentWidthKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(segmentWidthKey, segmentWidth);
        }
        else
        {
            segmentWidth = settings[segmentWidthKey];
        }

        UpdateSpecificSettings();
    }

    protected override void PreMakingVisualizer()
    {
        // 
    }
}