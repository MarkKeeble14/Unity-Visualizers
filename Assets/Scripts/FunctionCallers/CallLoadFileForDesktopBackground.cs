using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallLoadFileForDesktopBackground : MonoBehaviour
{
    public void LoadFile()
    {
        Cursor.visible = true;
        StartCoroutine(VisualizerManager._Instance.RunBackgroundSelection(sprite =>
        {
            if (sprite != null)
            {
                int whiteIndex = -1;
                for (int i = 0; i < VisualizerManager._Instance.TrackInfo.Colors.Count; i++)
                {
                    if (VisualizerManager._Instance.GetColor(VisualizerColorType.COLOR, i) == Color.white)
                    {
                        whiteIndex = i;
                        break;
                    }
                }
                if (whiteIndex >= 0)
                {
                    VisualizerElementsSettings cur = VisualizerManager._Instance.GetVisualizerElementSettings(VisualizerElementLabel.BACKGROUND);
                    cur.ColorIndex = whiteIndex;
                    VisualizerManager._Instance.UpdateVisualizerElementSettings(VisualizerElementLabel.BACKGROUND, cur);
                }
            }
            Cursor.visible = false;
        }));
    }
}
