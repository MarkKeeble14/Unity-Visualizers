using System.Collections.Generic;
using UnityEngine;

public class SetParticleSystemEnabledFromVisualizerElementInfo : DatabaseSetter, IRecieveVisualizerElementsInfo
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private ParticleSystem system;

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        TrySet(info, label, x =>
        {
            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = x.Enabled;
        });
    }
}
