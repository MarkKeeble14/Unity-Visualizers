using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomSetter : DatabaseSetter
{
    [SerializeField] private Volume volume;
    protected Bloom bloom;

    private void Awake()
    {
        volume.profile.TryGet(out bloom);
    }
}
