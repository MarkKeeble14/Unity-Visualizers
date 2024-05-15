using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "MinMaxBarReadFrom", menuName = "UI/MinMaxBarReadFrom", order = 1)]
public class MinMaxBarReadFrom : BarReadFrom
{
    [SerializeField] private float v;
    public float Value
    {
        get { return v; }
        set { v = value; }
    }
    [SerializeField] private float max;
    public float Max
    {
        get { return max; }
        set { max = value; }
    }

    public void Set(float min, float max)
    {
        this.v = min;
        this.max = max;
    }
}
