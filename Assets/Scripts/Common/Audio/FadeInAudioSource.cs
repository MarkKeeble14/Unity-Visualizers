using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private float targetVol;
    [SerializeField] private float rate = 2;

    // Update is called once per frame
    void Update()
    {
        if (sources[0].volume < targetVol)
        {
            foreach (AudioSource source in sources)
            {
                source.volume = Mathf.MoveTowards(source.volume, targetVol, Time.deltaTime * rate);
            }
        }
    }
}
