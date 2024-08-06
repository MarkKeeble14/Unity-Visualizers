using System.Collections.Generic;
using UnityEngine;

public class SetPremadeVisualizerSettings : SetValueFromVisualizerDatabase, IRecieveVisualizerFloatValues, IRecieveVisualizerIntValues
{
    [SerializeField] private PremadeVisualizer premadeVisualizer;

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        float defaultValue = 0, signalMultiplier = 0, adjustSpeed = 0;
        TrySetValueFromDatabase(SettingType.DEFAULT_VALUE, values, x => defaultValue = x);
        TrySetValueFromDatabase(SettingType.SIGNAL_MULTIPLIER, values, x => signalMultiplier = x);
        TrySetValueFromDatabase(SettingType.ADJUST_SPEED, values, x => adjustSpeed = x);

        premadeVisualizer.SetFloatSettings(signalMultiplier, defaultValue, adjustSpeed);
    }

    public void RecieveVisualizerIntValues(Dictionary<string, int> values)
    {
        TrySetValueFromDatabase(SettingType.VISUALIZER_SEGMENT_TYPE, values, x => premadeVisualizer.AttachmentType = (AttachmentType)x);
    }
}
