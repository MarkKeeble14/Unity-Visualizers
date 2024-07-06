using UnityEngine;

public class EstimatedTempoBroadcaster : SignalBroadcaster, IRecieveTempo
{
    [SerializeField] private float minBPMToActivate = 50;
    private float estimatedBPM;
    private float nextBeatTimer;
    private bool tick;

    public override float GetBroadcastValue()
    {
        if (tick) { tick = false; return 1; }
        return 0;
    }

    private void Update()
    {
        if (estimatedBPM <= minBPMToActivate) { tick = false; return; }

        if (nextBeatTimer < 0)
        {
            tick = true;
            SetNextBeatTimer();
        } else { nextBeatTimer -= Time.deltaTime; }
    }

    private void SetNextBeatTimer()
    {
        nextBeatTimer = 60 / estimatedBPM;
    }

    public void RecieveTempo(float tempo)
    {
        estimatedBPM = tempo;
        SetNextBeatTimer();
    }
}