using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachTimeOfDay : AttachParameter
{
    protected override void SetParameter(float value)
    {
        DayNightCycleManager._Instance.SetTimeOfDay(value);
    }
}
