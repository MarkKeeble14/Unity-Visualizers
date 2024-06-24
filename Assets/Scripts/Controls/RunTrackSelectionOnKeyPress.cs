using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTrackSelectionOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        StartCoroutine(VisualizerManager._Instance.RunTrackSelection());
    }
}
