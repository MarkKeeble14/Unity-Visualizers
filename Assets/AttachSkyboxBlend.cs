using UnityEngine;

public class AttachSkyboxBlend : AttachParameter
{
    [SerializeField] private SkyboxBlender blender;

    protected override void SetParameter(float value)
    {
        blender.blend = value;
    }
}
