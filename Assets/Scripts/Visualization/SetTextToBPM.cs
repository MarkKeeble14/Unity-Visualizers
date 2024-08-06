using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextToBPM : MonoBehaviour, IRecieveBPM
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int roundTo = 2;

    public void RecieveBPM(float bpm)
    {
        text.text = System.Math.Round(bpm, roundTo).ToString();
    }
}
