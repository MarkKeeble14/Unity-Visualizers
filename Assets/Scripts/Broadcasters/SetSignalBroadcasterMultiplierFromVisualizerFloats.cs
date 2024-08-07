using System.Collections.Generic;
using UnityEngine;

public class SetSignalBroadcasterMultiplierFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    [SerializeField] private SignalBroadcaster broadcaster;
    [SerializeField] private SettingType settingLabel;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, settingLabel.ToString(), x => broadcaster.SignalMultiplier = x);
    }
}
