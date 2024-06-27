using UnityEngine;

public static class RuntimeJsonSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void ApplyCustomConverters()
    {
        JsonCustomSettings.ConfigureJsonInternal();
    }
}
