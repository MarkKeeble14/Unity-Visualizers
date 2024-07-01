using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RandomHelper
{
    private static char[] numbers = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static char[] alphabet = new char[26] { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };
    private static string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private static readonly Dictionary<char, KeyCode> _keycodeCache = new Dictionary<char, KeyCode>();
    public static KeyCode GetRandomKeyCode(bool includeNums, bool includeAlphabet)
    {
        List<char> options = new List<char>();
        if (includeNums)
        {
            options.AddRange(numbers);
        }
        if (includeAlphabet)
        {
            options.AddRange(alphabet);
        }
        return GetKeyCodeFromOptions(options);
    }


    public static KeyCode GetKeyCodeFromOptions(List<char> options)
    {
        int r = UnityEngine.Random.Range(0, options.Count);
        char character = options[r];

        // Get from cache if it was taken before to prevent unnecessary enum parse
        KeyCode code;
        if (_keycodeCache.TryGetValue(character, out code)) return code;

        // Cast to it's integer value
        int alphaValue = character;
        code = (KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());
        _keycodeCache.Add(character, code);
        return code;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float RandomFloat(float minInclusive, float maxInclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    public static float RandomFloat(Vector2 minMax)
    {
        return RandomFloat(minMax.x, minMax.y);
    }

    public static int RandomIntExclusive(Vector2 minMax)
    {
        return UnityEngine.Random.Range((int)minMax.x, (int)minMax.y);
    }

    public static int RandomIntExclusive(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static int RandomIntInclusive(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static int RandomIntInclusive(Vector2 minMax)
    {
        return UnityEngine.Random.Range((int)minMax.x, (int)minMax.y + 1);
    }

    public static int RandomIntInclusive(Vector2Int minMax)
    {
        return RandomIntInclusive(minMax.x, minMax.y + 1);
    }

    public static int RandomIntExclusive(Vector2Int minMax)
    {
        return RandomIntExclusive(minMax.x, minMax.y);
    }

    public static T GetRandomFromList<T>(List<T> list)
    {
        return list[RandomIntExclusive(0, list.Count)];
    }

    public static bool RandomBool()
    {
        return UnityEngine.Random.value <= 0.5f;
    }

    public static bool EvaluateChanceTo(Vector2 chanceTo)
    {
        return EvaluateChanceTo(chanceTo.x, chanceTo.y);
    }

    public static bool EvaluateChanceTo(float x, float y)
    {
        if (x == 0) return false;
        return (UnityEngine.Random.value * y) >= (y - x);
    }

    public static char GetRandomAlphanumericCharacter()
    {
        return alphanumericChars[RandomIntExclusive(0, alphanumericChars.Length)];
    }

    public static string GetRandomAlphanumericString(int length)
    {
        string result = "";
        for (int i = 0; i < length; ++i)
        {
            result += GetRandomAlphanumericCharacter();
        }
        return result;
    }
}