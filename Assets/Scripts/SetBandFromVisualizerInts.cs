using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBandFromVisualizerInts : SetValueFromVisualizerDatabase, IRecieveVisualizerIntValues
{
    [SerializeField] private BandBroadcaster bandBroadcaster;

    public void RecieveVisualizerIntValues(Dictionary<string, int> values)
    {
        TrySetValueFromDatabase(SettingType.ATTACHED_TO_BAND, values, x => bandBroadcaster.Band = x);
    }
}
