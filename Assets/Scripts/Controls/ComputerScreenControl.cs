using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComputerScreenControl : MonoBehaviour
{
    private static List<ComputerScreenControl> currentlyHovered = new();
    public static int NumHovered => currentlyHovered.Count;

    [SerializeField] private int priority;
    public int Priority => priority;

    [SerializeField] private RectTransform rect;
    public RectTransform Rect => rect;

    protected Action whileHovered;
    protected Action whileNotHovered;
    protected Action clicked;
    protected Action released;
    protected Action held;

    public bool IsHeld { get; private set; }
    public bool Disabled { get; set; }

    private void Awake()
    {
        clicked += () => IsHeld = true;
        released += () => IsHeld = false;
        LoadEvents();
    }

    protected abstract void LoadEvents();

    public void Hovered()
    {
        if (!currentlyHovered.Contains(this))
        {
            currentlyHovered.Add(this);
        }

        whileHovered?.Invoke();
    }

    public void NotHovered()
    {
        if (currentlyHovered.Contains(this))
        {
            currentlyHovered.Remove(this);
        }

        whileNotHovered?.Invoke();
    }

    public void Clicked()
    {
        clicked?.Invoke();
    }

    public void Released()
    {
        released?.Invoke();
    }

    public void Held()
    {
        held?.Invoke();
    }
}