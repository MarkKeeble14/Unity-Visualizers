using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class LongLineVisualizer : PremadeVisualizer
{
    [Header("Settings")]
    [SerializeField] private float spacing = 0;

    private HorizontalOrVerticalLayoutGroup layoutGroup;

    protected override void UpdateSpecificSettings()
    {
        layoutGroup.spacing = spacing;
    }

    protected override void PreMakingVisualizer()
    {
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }
}