using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageToBackground : MonoBehaviour, IRecieveTrackInfo
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void IRecieveTrackInfo.RecieveTrackInfo(TrackInfo info)
    {
        if (image == null) return;
        image.sprite = info.Background;
    }
}