using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDayNightTimeScaleFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, SettingType.DAY_NIGHT_TIME_SCALE.ToString(), x => DayNightCycleManager._Instance.TimeScale = x);
    }
}
