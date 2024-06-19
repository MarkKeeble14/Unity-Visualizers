using UnityEngine;

public class SeekOnKeyPressed : ActWhileKeyDown
{
    [SerializeField] private float seekSpeed;
    [SerializeField] private SeekDirection seekDir;
    private float speedMultiplier = 1;

    public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }

    protected override void Act()
    {
        VisualizerManager._Instance.Seek(seekSpeed * (int)seekDir * speedMultiplier);
    }
}
