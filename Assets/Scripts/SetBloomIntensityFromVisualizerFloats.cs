using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetBloomIntensityFromVisualizerFloats : BloomSetter, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(VisualizerElementLabel.BLOOM.ToString(), SettingType.BLOOM_INTENSITY.ToString()), x => bloom.intensity.Override(x));
    }
}
