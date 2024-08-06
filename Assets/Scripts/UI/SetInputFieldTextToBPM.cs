using TMPro;
using UnityEngine;

public class SetInputFieldTextToBPM : MonoBehaviour, IRecieveBPM
{
    [SerializeField] private TMP_InputField text;
    [SerializeField] private int roundTo = 2;

    public void RecieveBPM(float bpm)
    {
        text.text = System.Math.Round(bpm, roundTo).ToString();
    }
}
