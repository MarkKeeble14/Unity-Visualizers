using System.Collections.Generic;
using UnityEngine;

public class SetLightIntensityFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    [SerializeField] private Light light;
    [SerializeField] private VisualizerElementLabel key;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, MakeKey(key.ToString(), SettingType.LIGHT_INTENSITY.ToString()), x => light.intensity = x);
    }
}
