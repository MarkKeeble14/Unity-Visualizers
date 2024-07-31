using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFogNormalizerValuesFromVisualizerFloats : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private NormalizeBroadcastValue normalizer;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(SettingType.MIN_AUDIO_INPUT, values, x => normalizer.MinimumInput = x);
        TrySetValueFromDatabase(SettingType.MAX_AUDIO_INPUT, values, x => normalizer.MaximumInput = x);
        TrySetValueFromDatabase(SettingType.MIN_NORMALIZED_OUTPUT, values, x => normalizer.MinimumOutput = x);
        TrySetValueFromDatabase(SettingType.MAX_NORMALIZED_OUTPUT, values, x => normalizer.MaximumOutput = x);
    }
}
