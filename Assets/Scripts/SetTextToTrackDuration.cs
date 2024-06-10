using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SetTextToTrackDuration : MonoBehaviour, IRecieveTrackInfo
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void IRecieveTrackInfo.RecieveTrackInfo(TrackInfo info)
    {
        text.text = info.Duration;
    }
}
