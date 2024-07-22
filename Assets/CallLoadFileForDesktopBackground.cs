using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallLoadFileForDesktopBackground : MonoBehaviour
{
    [SerializeField] private Image i;

    public void LoadFile()
    {
        Cursor.visible = true;
        StartCoroutine(VisualizerManager._Instance.BrowseForImageFile(image =>
        {
            i.color = Color.white;
            i.sprite = image;
        }, () => Cursor.visible = false));
    }
}
