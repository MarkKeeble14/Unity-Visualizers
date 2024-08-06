using System.Collections.Generic;
using UnityEngine;

public class SetLongLineSpacingFromVisualizerFloats : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private LongLineVisualizer llVisualizer;
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(SettingType.SPACING, values, x => llVisualizer.Spacing = x);
    }
}
