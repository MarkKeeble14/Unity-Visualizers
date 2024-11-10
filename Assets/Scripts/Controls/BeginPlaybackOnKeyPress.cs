using System;
using System.Collections;
using System.Collections.Generic;

public class BeginPlaybackOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        VisualizerManager._Instance.BeginPlayback();
    }
}
