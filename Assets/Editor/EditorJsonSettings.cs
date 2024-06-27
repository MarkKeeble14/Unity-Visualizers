using UnityEditor;
// this must be inside an Editor/ folder
public static class EditorJsonSettings
{
    [InitializeOnLoadMethod]
    public static void ApplyCustomConverters()
    {
        JsonCustomSettings.ConfigureJsonInternal();
    }
}
