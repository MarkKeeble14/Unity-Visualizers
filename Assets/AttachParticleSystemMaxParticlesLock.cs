using UnityEngine;

public class AttachParticleSystemMaxParticlesLock : AttachParameterLock
{
    [SerializeField] private ParticleSystem system;
    private ParticleSystem.MainModule main;
    private float value;

    private void Awake()
    {
        main = system.main;
        currentValue = main.maxParticles;
    }

    protected override void InstantUnlock()
    {
        main.maxParticles = Mathf.FloorToInt(unlockedValue);
    }

    protected override void UpdateValue()
    {
        value = GetNextValue(value);
        main.maxParticles = Mathf.FloorToInt(value);
    }

    protected override bool HasReachedFinalValue()
    {
        return main.maxParticles == unlockedValue;
    }
}
