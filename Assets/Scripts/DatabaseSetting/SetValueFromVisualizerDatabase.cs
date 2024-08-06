using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SetValueFromVisualizerDatabase : MonoBehaviour
{
    [SerializeField] private VisualizerElementLabel elementLabel;

    protected bool TrySetValueFromDatabase<T>(SettingType settingType, Dictionary<string, T> db, Action<T> setFunc, Action notFoundFunc)
    {
        string dbKey = MakeKey(settingType);
        if (db.ContainsKey(dbKey)) { setFunc?.Invoke(db[dbKey]); return true; } else { notFoundFunc?.Invoke(); return false; }
    }

    protected bool TrySetValueFromDatabase<T>(SettingType settingType, Dictionary<string, T> db, Action<T> setFunc)
    {
        string dbKey = MakeKey(settingType);
        if (db.ContainsKey(dbKey)) { setFunc?.Invoke(db[dbKey]); return true; } else { Debug.Log("key=" + dbKey + " - Not Found in Database"); return false; }
    }

    private string MakeKey(SettingType settingType)
    {
        return elementLabel + "_" + settingType;
    }
}
