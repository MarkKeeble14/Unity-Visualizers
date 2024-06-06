using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RadialLayoutGroup))]
public class RadialVisualizer : PremadeVisualizer
{
    [Header("Settings")]
    [SerializeField] private float distance = 0;

    private RadialLayoutGroup layoutGroup;

    protected override void UpdateSpecificSettings()
    {
        layoutGroup.fDistance = distance;
    }

    protected override void PreMakingVisualizer()
    {
        layoutGroup = GetComponent<RadialLayoutGroup>();
    }
}