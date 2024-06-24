using UnityEngine;

public class AttachParticleSystemEmissionRateOverTime : AttachParameter
{
    [SerializeField] private ParticleSystem system;
    private ParticleSystem.EmissionModule emission;

    private void Awake()
    {
        emission = system.emission;
    }

    protected override void SetParameter(float value)
    {
        emission.rateOverTime = value;
    }
}
