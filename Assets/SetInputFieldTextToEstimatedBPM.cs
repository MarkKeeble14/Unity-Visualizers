using TMPro;
using UnityEngine;

public class SetInputFieldTextToEstimatedBPM : MonoBehaviour, IRecieveTempo
{
    [SerializeField] private TMP_InputField text;
    [SerializeField] private int roundTo = 2;

    public void RecieveTempo(float tempo)
    {
        text.text = System.Math.Round(tempo, roundTo).ToString();
    }
}
