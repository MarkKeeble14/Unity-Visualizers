using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMonoBehaviourEnabledFromVisualizerElementsInfo : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private MonoBehaviour behaviour;
    [SerializeField] private VisualizerElementLabel key;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, key, x => behaviour.enabled = x.Enabled);
    }
}
