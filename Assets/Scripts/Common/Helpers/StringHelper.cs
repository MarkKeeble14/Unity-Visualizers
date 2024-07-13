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

    public static string GetDurationText(float totalSeconds)
    {
        float minutes = Mathf.FloorToInt(totalSeconds / 60);
        float seconds = Mathf.FloorToInt(totalSeconds - (minutes * 60));
        return minutes + ":" + (seconds >= 10 ? seconds : "0" + seconds);
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public static string GetFileExtension(string filePath)
    {
        string extension = "";
        for (int i = filePath.Length - 1; i > 0; --i)
        {
            if (filePath[i].Equals('.'))
                break;
            extension += filePath[i];
        }
        return Reverse(extension);
    }

    public static string GetFileName(string filePath)
    {
        string pathWithoutExtension = filePath.Split(GetFileExtension(filePath))[0];
        string fileName = "";
        for (int i = pathWithoutExtension.Length - 2; i >= 0; --i)
        {
            char c = pathWithoutExtension[i];
            if (c.Equals('\\'))
                break;
            fileName += c;
        }
        return Reverse(fileName);
    }
}
