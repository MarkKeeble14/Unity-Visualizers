using System;
using UnityEngine;

public class UncaughtSwitchTypeException : Exception
{
    public UncaughtSwitchTypeException(Type enumType)
    {
        Debug.LogError("Uncaught Type: " + enumType.ToString());
    }
}