using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachParticleSystemMaxParticles : AttachParameter
{
    [SerializeField] private ParticleSystem system;
    private ParticleSystem.MainModule main;

    private void Awake()
    {
        main = system.main;
    }

    protected override void SetParameter(float value)
    {
        main.maxParticles = Mathf.RoundToInt(value);
    }
}
