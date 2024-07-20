using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SetTextToTrackTitle : MonoBehaviour, IRecieveTrackInfo
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void IRecieveTrackInfo.RecieveTrackInfo(TrackInfo info)
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        text.text = info.Title;
    }
}
