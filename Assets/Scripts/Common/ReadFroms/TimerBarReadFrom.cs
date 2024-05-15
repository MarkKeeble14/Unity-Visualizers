using UnityEngine;

[CreateAssetMenu(fileName = "TimerBarReadFrom", menuName = "UI/TimerBarReadFrom", order = 1)]
public class TimerBarReadFrom : BarReadFrom
{
    private float timer;
    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }
    private float duration;
    public float Duration
    {
        get { return duration; }
    }

    public void Initialize(float time)
    {
        duration = time;
        timer = 0;
    }

    public void Update()
    {
        timer += Time.deltaTime;
    }
}