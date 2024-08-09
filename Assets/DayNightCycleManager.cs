using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    public static DayNightCycleManager _Instance { get; private set; }

    private float elapsedTime;
    [SerializeField] private float timeScale = 1;
    [SerializeField, Range(0, 24)] private int startHour = 6;
    public float CurrentHourExact => elapsedTime / 60;
    public int CurrentHour => Mathf.FloorToInt(CurrentHour);
    public float CurrentHourOfDay => CurrentHourExact % 24;
    public float PercentThroughDay => CurrentHourOfDay / 24;

    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;
    }

    private void Start()
    {
        elapsedTime = startHour * 60;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;
    }

    public float GetTimeSinceHour(float hour)
    {
        return Mathf.Abs(CurrentHourExact % 24 - hour);
    }
}
