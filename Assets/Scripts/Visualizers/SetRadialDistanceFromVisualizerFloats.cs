using System.Collections.Generic;
using UnityEngine;

public class SetRadialDistanceFromVisualizerFloats : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private RadialVisualizer radialVisualizer;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(SettingType.RADIAL_DISTANCE, values, x => radialVisualizer.Distance = x);
    }
}
