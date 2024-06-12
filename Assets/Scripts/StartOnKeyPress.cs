using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOnKeyPress : MonoBehaviour
{
    [SerializeField] private KeyCode key = KeyCode.Return;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (Fader._Instance != null)
            {
                Fader._Instance.FadeOutBlocker();
            }
            VisualizerManager._Instance.BeginPlayback();
        }
    }
}
