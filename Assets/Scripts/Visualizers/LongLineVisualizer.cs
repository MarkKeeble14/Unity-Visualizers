using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class LongLineVisualizer : PremadeVisualizer, IRecieveVisualizerFloatValues
{
    [Header("Settings")]
    [SerializeField] private float spacing = 0;

    [SerializeField] private string spacingKey;

    private HorizontalOrVerticalLayoutGroup layoutGroup;

    protected override void PreMakingVisualizer()
    {
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    protected override void UpdateSpecificSettings()
    {
        layoutGroup.spacing = spacing;
    }

    public override void RecieveVisualizerFloatValues(Dictionary<string, float> settings)
    {
        base.RecieveVisualizerFloatValues(settings);

        if (!settings.ContainsKey(spacingKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(spacingKey, spacing);
        } else
        {
            spacing = settings[spacingKey];
        }
    }
}