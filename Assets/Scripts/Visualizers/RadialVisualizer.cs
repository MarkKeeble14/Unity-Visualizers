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

    private RadialLayoutGroup layoutGroup;

    protected override void UpdateSpecificSettings()
    {
        // radial distance
        layoutGroup.fDistance = distance;

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

    protected override void PreMakingVisualizer()
    {
        layoutGroup = GetComponent<RadialLayoutGroup>();
    }
}