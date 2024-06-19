using UnityEngine;

public class ChangeSeekSpeedOnKeyPress : ActOnKeyPress
{
    [SerializeField] private SeekOnKeyPressed[] seekers;
    [SerializeField] private float limit;
    [SerializeField] private float speedIncrement;

    protected override void Act()
    {
        foreach (SeekOnKeyPressed seeker in seekers)
        {
            if (seeker.SpeedMultiplier != limit)
                seeker.SpeedMultiplier += speedIncrement;
        }

    }
}
