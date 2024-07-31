using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class VisualizerSetupHelperButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int index;

    public Button Button => button;
    public TextMeshProUGUI Text => text;
    public int Index => index;

    public Action<Button> ControlButtonInteractable;

    private void Update()
    {
        if (ControlButtonInteractable != null) { ControlButtonInteractable.Invoke(button); }
    }
}
