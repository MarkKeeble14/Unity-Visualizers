using UnityEngine;

public class ChangeVolumeWhileKeyDown : ActWhileKeyDown
{
    [SerializeField] private float changeSpeed;
    [SerializeField] private int directionMultiplier = 1;

    protected override void Act()
    {
        VisualizerManager._Instance.ChangeVolume(changeSpeed * Time.deltaTime * directionMultiplier);
    }
}
