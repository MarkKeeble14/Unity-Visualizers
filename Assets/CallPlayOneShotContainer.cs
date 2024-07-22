using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPlayOneShotContainer : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private bool randomizeVolume;
    [SerializeField] private bool randomizePitch;

    public void PlayOneShot()
    {
        SFXManager._Instance.PlayOneShot(key, randomizeVolume, randomizePitch);
    }
}

