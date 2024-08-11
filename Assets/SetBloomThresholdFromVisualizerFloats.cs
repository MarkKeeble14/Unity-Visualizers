using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBloomThresholdFromVisualizerFloats : BloomSetter, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(VisualizerElementLabel.BLOOM.ToString(), SettingType.BLOOM_THRESHOLD.ToString()), x => bloom.threshold.Override(x));
    }
}
