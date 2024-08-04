using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RadialLayoutGroup))]
public class RadialVisualizer : PremadeVisualizer
{
    [Header("Settings")]
    [SerializeField] private float distance = 0;
    public float Distance { get { return distance; } set { distance = value; } }
    [SerializeField] private float segmentWidth = 10;
    public float SegmentWidth { get { return segmentWidth; } set { segmentWidth = value; UpdateSegmentWidth(); } }

    private RadialLayoutGroup layoutGroup;

    protected override void UpdateSpecificSettings()
    {
        // radial distance
        layoutGroup.UpdateDistance(distance);
    }

    public void UpdateSegmentWidth()
    {
        Vector2 newDelta = new Vector2(segmentWidth, 0);
        foreach (RectTransform rect in segmentList) { rect.sizeDelta = newDelta; }
    }

    protected override void PreMakingVisualizer()
    {
        if (layoutGroup == null) { layoutGroup = GetComponent<RadialLayoutGroup>(); }
    }

    protected override void PostMakingVisualizer()
    {
        UpdateSegmentWidth();
    }
}