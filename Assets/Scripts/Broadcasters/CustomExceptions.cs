using System;
using UnityEngine;

public class UncaughtSwitchTypeException : Exception
{
    public UncaughtSwitchTypeException(Type enumType)
    {
        Debug.LogError("Uncaught Type: " + enumType.ToString());
    }
}

public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException(string extension)
    {
        Debug.LogError("Invalid File Type - Extension " + extension + " not supported for this operation");
    }
}