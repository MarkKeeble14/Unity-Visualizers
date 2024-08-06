using System.Collections.Generic;
using UnityEngine;

public class SetRadialSegmentWidthFromVisualizerFloats : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private RadialVisualizer radialVisualizer;
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(SettingType.SEGMENT_WIDTH, values, x => radialVisualizer.SegmentWidth = x);
    }
}
