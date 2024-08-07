using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DatabaseSetter : MonoBehaviour
{
    protected string MakeKey(string elementLabel, string settingType)
    {
        return elementLabel + "_" + settingType;
    }

    protected void TrySet<X, Y>(Dictionary<X, Y> database, X key, Action<Y> setFunc)
    {
        if (!database.ContainsKey(key)) return;
        setFunc?.Invoke(database[key]);
    }
}
