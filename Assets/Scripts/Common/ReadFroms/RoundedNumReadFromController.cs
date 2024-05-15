using UnityEngine;
using TMPro;

public class RoundedNumReadFromController : MonoBehaviour
{
    [SerializeField] private NumReadFrom rf;
    [SerializeField] private string label;
    [SerializeField] private int roundTo;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = label + ": " + System.Math.Round(rf.Value, roundTo);
    }
}