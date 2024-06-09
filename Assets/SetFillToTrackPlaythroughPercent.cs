using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetFillToTrackPlaythroughPercent : MonoBehaviour
{
    private Image toFill;

    private void Awake()
    {
        toFill = GetComponent<Image>();
    }

    private void Update()
    {
        toFill.fillAmount = VisualizerManager._Instance.PlaythroughPercent;
    }
}
