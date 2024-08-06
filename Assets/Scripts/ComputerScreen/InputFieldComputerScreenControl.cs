using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldComputerScreenControl : ComputerScreenControl
{
    [SerializeField] private TMP_InputField inputField;

    protected override void LoadEvents()
    {
        clicked += () =>
        {
            if (!inputField.interactable) return;
            inputField.Select();
        };
    }
}
