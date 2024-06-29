using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }

    [SerializeField] private InputFieldPopup inputFieldDialog;

    public void PopupInputField(string directions, string confirmButtonText, string cancelButtonText, Action<string> onSuccess, Action onFailure)
    {
        inputFieldDialog.gameObject.SetActive(true);
        inputFieldDialog.Open(directions, confirmButtonText, cancelButtonText, onSuccess, onFailure);
    }

    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;
    }
}