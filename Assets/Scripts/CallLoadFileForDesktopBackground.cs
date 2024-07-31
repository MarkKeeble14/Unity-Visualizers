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
        StartCoroutine(VisualizerManager._Instance.RunCoverArtSelection(sprite =>
        {
            if (sprite != null)
            {
                VisualizerElementsSettings cur = VisualizerManager._Instance.GetBaseVisualizerElementSettings(VisualizerElementLabel.BACKGROUND);
                cur.ColorIndex = 0;
                VisualizerManager._Instance.UpdateBaseVisualizerElementSettings(VisualizerElementLabel.BACKGROUND, cur);

                i.sprite = sprite;
            }
            Cursor.visible = false;
        }));
    }
}
