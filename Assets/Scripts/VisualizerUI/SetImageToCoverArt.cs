using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageToCoverArt : MonoBehaviour, IRecieveTrackInfo
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void IRecieveTrackInfo.RecieveTrackInfo(TrackInfo info)
    {
        if (image == null) return;
        image.sprite = info.CoverArt;
    }
}
