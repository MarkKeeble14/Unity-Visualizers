using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class ColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        JArray array = new(value.r, value.g, value.b, value.a);
        array.WriteTo(writer);
    }

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JArray array = JArray.Load(reader);
        return new Color((float)array[0], (float)array[1], (float)array[2], (float)array[3]);
    }
}

public class FontConverter : JsonConverter<Font>
{
    public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
    {
        string str = "{ " +
            "Ascent: '" + value.ascent + "'," +
            "CharacterInfo: '" + value.characterInfo + "'," +
            "Dynamic: '" + value.dynamic + "'," +
            "FontNames: '" + value.fontNames + "'," +
            "FontSize: '" + value.fontSize + "'," +
            "HideFlags: '" + value.hideFlags + "'," +
            "LineHeight: '" + value.lineHeight + "'," +
            "Material: '" + value.material + "'," +
            "Name: '" + value.name + "'" +
        "}";

        JObject o = JObject.Parse(str);
        o.WriteTo(writer);
    }

    public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject o = JObject.Load(reader);

        Font f = new Font();

        /*
        JToken ascent;
        JToken charInfo;
        JToken dynamic;
        JToken fontNames;
        JToken fontSize;
        JToken hideFlags;
        JToken lineHeight;
        JToken material;
        JToken name;

        o.TryGetValue("Ascent", out ascent);
        o.TryGetValue("CharacterInfo", out charInfo);
        o.TryGetValue("Dynamic", out dynamic);
        o.TryGetValue("FontNames", out fontNames);
        o.TryGetValue("FontSize", out fontSize);
        o.TryGetValue("HideFlags", out hideFlags);
        o.TryGetValue("LineHeight", out lineHeight);
        o.TryGetValue("Material", out material);
        o.TryGetValue("Name", out name);
        */

        return f;
    }
}