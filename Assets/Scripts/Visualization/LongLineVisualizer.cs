using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class LongLineVisualizer : PremadeVisualizer
{
    [Header("Settings")]
    [SerializeField] private float spacing = 0;
    public float Spacing {  get { return spacing; } set { spacing = value; } }

    private HorizontalOrVerticalLayoutGroup layoutGroup;

    protected override void PreMakingVisualizer()
    {
        if (layoutGroup == null) { layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>(); }
    }

    protected override void PostMakingVisualizer()
    {
        // 
    }

    protected override void UpdateSpecificSettings()
    {
        layoutGroup.spacing = spacing;
    }
}