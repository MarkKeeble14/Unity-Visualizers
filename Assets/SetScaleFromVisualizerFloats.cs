using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleFromVisualizerFloats : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues
{
    [SerializeField] private RectTransform rect;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        TrySetValueFromDatabase(SettingType.BACKGROUND_SCALE, values, x => rect.localScale = Vector3.one * x);
    }
}
