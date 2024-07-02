using Newtonsoft.Json;

public static class JsonCustomSettings
{
    public static void ConfigureJsonInternal()
    {
        JsonConvert.DefaultSettings = () =>
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ColorConverter());
            settings.Converters.Add(new FontConverter());
            return settings;
        };
    }
}
