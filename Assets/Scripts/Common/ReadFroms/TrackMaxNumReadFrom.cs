using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackMaxNumReadFrom", menuName = "UI/TrackMaxNumReadFrom", order = 1)]
public class TrackMaxNumReadFrom : NumReadFrom
{
    private float max;
    public float Max
    {
        get { return max; }
        set { max = value; }
    }

    public new float Value
    {
        get { return base.Value; }
        set
        {
            if (value > max)
            {
                max = value;
            }
            base.Value = value;
        }
    }

    public override void Reset()
    {
        max = 0;
        base.Reset();
    }
}
