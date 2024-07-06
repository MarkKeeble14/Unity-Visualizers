using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLightColorToTrackColor : SetElementColorToMatchTrack, IRecieveTrackInfo
{
    [SerializeField] private Light light; 

    protected override void SetElementToColor(Color c)
    {
        light.color = c;
    }
}
