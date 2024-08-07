using System.Collections.Generic;
using UnityEngine;

public class SetBouncingFloatBroadcasterSpeed : DatabaseSetter, IRecieveVisualizerFloatValues
{
    [SerializeField] private BouncingFloatBroadcaster broadcaster;
    [SerializeField] private SettingType settingType;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, settingType.ToString(), x =>
        {
            broadcaster.Speed = x;
            broadcaster.ResetValues();
        });
    }
}
