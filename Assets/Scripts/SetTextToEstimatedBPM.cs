using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextToEstimatedBPM : MonoBehaviour, IRecieveTempo
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int roundTo = 2;

    public void RecieveTempo(float tempo)
    {
        text.text = System.Math.Round(tempo, roundTo).ToString();
    }
}
