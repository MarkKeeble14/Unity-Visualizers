using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper
{
    public static string ToDetailedString(this Vector2 v)
    {
        return System.String.Format("{0}, {1}", v.x, v.y);
    }

    public static string ToDetailedString(this float v)
    {
        return System.String.Format("{0}", v);
    }

    public static string GetDurationText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time - (minutes * 60));
        return minutes + ":" + (seconds >= 10 ? seconds : "0" + seconds);
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
