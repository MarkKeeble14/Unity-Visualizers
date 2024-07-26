using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UncaughtSwitchTypeException : Exception
{
    public UncaughtSwitchTypeException(Type enumType, string attemptedType)
    {
        Debug.LogError("Uncaught Type: " + attemptedType + " (" + enumType.ToString() + ")");
    }
}

public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException(string extension)
    {
        Debug.LogError("Invalid File Type - Extension " + extension + " not supported for this operation");
    }
}

public class ElementWithKeyNotFoundException : Exception
{
    public ElementWithKeyNotFoundException(Type elementType, string key)
    {
        Debug.LogError("No Element found with key=" + key + " (" + elementType.ToString() + ")");
    }
}

public class VisualizerSceneNotSelectedException : Exception
{
    public VisualizerSceneNotSelectedException()
    {
        Debug.LogError("Expected to have selected a scene at the time of function call");
    }
}

public class MalformedKeyControlsForControlElementException : Exception
{
    public MalformedKeyControlsForControlElementException(int passedLength, int requiredLength)
    {
        Debug.LogError("Key Controls array does not match size needed for specified control element " +
            "- Array Length=" + passedLength + ", Required Length=" + requiredLength);
    }
}

public class AllPartOfThePlanException : Exception
{
    public AllPartOfThePlanException()
    {
        Debug.Log("All part of the plan captain");
    }
}

public class IndexNotFoundException<T> : Exception
{
    public IndexNotFoundException(int index, Dictionary<T, int> dict)
    {
        Debug.LogError("Index requested was not found within options - Requested Index=" + index + ", " +
            "Available Indices=" + StringHelper.CombineCollection(dict.Values.ToArray(), ", "));
    }
}