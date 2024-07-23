using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteAudioSourceByCheckBox : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private Checkbox checkbox;

    // Update is called once per frame
    void Update()
    {
        source.mute = checkbox.Active;
    }
}
