using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class TimerDictionary<X> : Dictionary<X, float>
{
    private List<X> garbage = new List<X>();

    public Action<X> OnAddItem;
    public Action<X> OnRemoveItem;

    private List<X> matchedKeys = new();

    public new void Add(X item, float duration)
    {
        OnAddItem?.Invoke(item);
        base.Add(item, duration);
        matchedKeys = Keys.ToList();
    }

    public new void Remove(X item)
    {
        OnRemoveItem?.Invoke(item);
        base.Remove(item);
        matchedKeys = Keys.ToList();
    }

    public void Update()
    {
        foreach (X key in matchedKeys)
        {
            this[key] -= Time.deltaTime;

            if (this[key] < 0)
            {
                garbage.Add(key);
            }
        }

        while (garbage.Count > 0)
        {
            Remove(garbage[0]);
            garbage.RemoveAt(0);
        }
    }
}
