using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeOfDayFromVisualizerFloats : DatabaseSetter, IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySet(values, SettingType.TIME_OF_DAY.ToString(), x => DayNightCycleManager._Instance.SetTimeOfDay(x));
    }
}
