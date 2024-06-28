using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImageActiveOnVisualizerElementActive : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Image image;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        image.enabled = info[label].Enabled;
    }
}