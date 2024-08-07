using UnityEngine;

public class ParticleSystemEmissionIntermediateFloatAttachment : IntermediateFloatAttachment
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