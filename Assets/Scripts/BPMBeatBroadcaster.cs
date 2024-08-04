using System.Collections;
using UnityEngine;

public class BPMBeatBroadcaster : SignalBroadcaster
{
    [SerializeField] private float outputOnBeat = 1;
    [SerializeField] private float outputOnNotBeat = 0;
    [SerializeField] private int beatsPerSignal = 1;
    [SerializeField] private float beatDuration = 0.25f;
    private float beatDurationTimer;
    private bool onBeat;
    private int signalsSinceLastBeat;

    private void Start()
    {
        VisualizerManager._Instance.OnBeat += () =>
        {
            signalsSinceLastBeat++;
            if (signalsSinceLastBeat >= beatsPerSignal)
            {
                onBeat = true;
                beatDurationTimer = beatDuration;
                signalsSinceLastBeat = 0;
            }
        };
    }

    public override float GetBroadcastValue()
    {
        if (onBeat)
        {
            if (beatDurationTimer <= 0)
            {
                onBeat = false;
                return outputOnNotBeat;
            } else
            {
                beatDurationTimer -= Time.deltaTime;
            }
            return outputOnBeat;
        } else
        {
            return outputOnNotBeat;
        }
    }
}