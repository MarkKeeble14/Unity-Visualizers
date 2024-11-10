using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSongInstructionsText : MonoBehaviour
{
    [SerializeField] private GameObject disableObject;

    // Start is called before the first frame update
    void Start()
    {
        VisualizerManager._Instance.OnSongStart += () => disableObject.SetActive(false);
    }
}
