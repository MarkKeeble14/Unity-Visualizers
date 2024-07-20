using System.Collections.Generic;
using UnityEngine;

public abstract class ComputerScreenControl : MonoBehaviour
{
    private static List<ComputerScreenControl> currentlyHovered = new();
    public static int NumHovered => currentlyHovered.Count;

    [SerializeField] private RectTransform rect;
    public RectTransform Rect => rect;
    public void Hovered()
    {
        if (!currentlyHovered.Contains(this))
        {
            currentlyHovered.Add(this);
        }

        WhileHovered();
    }

    public void NotHovered()
    {
        if (currentlyHovered.Contains(this))
        {
            currentlyHovered.Remove(this);
        }

        WhileNotHovered();
    }

    public abstract void WhileHovered();
    public abstract void WhileNotHovered();
    public abstract void Clicked();
}