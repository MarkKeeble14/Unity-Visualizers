using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEmissionIntermediateMinMaxFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private IntermediateFloatAttachment intermediate;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        float min = 0, max = 0;
        TrySet(values, MakeKey(label.ToString(), SettingType.MINIMUM_EMISSION.ToString()), x => { min = x; });
        TrySet(values, MakeKey(label.ToString(), SettingType.MAXIMUM_EMISSION.ToString()), x => { max = x; });
        intermediate.SetMinMaxValue(min, max);
    }
}
