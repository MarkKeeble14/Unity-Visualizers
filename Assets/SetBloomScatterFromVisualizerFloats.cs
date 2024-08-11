using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetBloomScatterFromVisualizerFloats : BloomSetter, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(VisualizerElementLabel.BLOOM.ToString(), SettingType.BLOOM_SCATTER.ToString()), x => bloom.scatter.Override(x));
    }
}
