using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLightColorFromVisualizerElementsInfo : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] private Light light;
    [SerializeField] private VisualizerElementLabel key;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        VisualizerManager._Instance.SetLightColorToVisualizerElement(key, light);
    }
}
