using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLightColorFromVisualizerSpecificSettings : MonoBehaviour, IRecieveVisualizerSpecificElementsInfo
{
    [SerializeField] private Light light;
    [SerializeField] private string key;

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        VisualizerManager._Instance.SetLightColorToVisualizerSpecificElementSettings(key, light);
    }
}
