using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterFreeCamOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        FreeCameraController._Instance.Activate();
    }
}
