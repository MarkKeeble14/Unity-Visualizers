public struct SignalBroadcastObject
{
    public int FrequencyId;
    public float Value;

    public SignalBroadcastObject(int frequencyId, float v) : this()
    {
        FrequencyId = frequencyId;
        Value = v;
    }
}
