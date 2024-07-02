using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }

    [SerializeField] private InputFieldPopup inputFieldDialog;
    [SerializeField] private ActionSelectionPopup actionSelectionPopup;

    [SerializeField] private TimerDictionary<string> popupMessageDict = new();

    [Header("References")]
    [SerializeField] private Transform popupMessagesList;

    [Header("Prefabs")]
    [SerializeField] private TextMeshProUGUI textPrefab;
    private Dictionary<string, TextMeshProUGUI> spawnedMessagesDict = new();

    private void Awake()
    {
        if (_Instance != null) { Destroy(_Instance.gameObject); }
        _Instance = this;

        popupMessageDict.OnAddItem += ShowMessage;
        popupMessageDict.OnRemoveItem += RemoveMessage;
    }

    private void Update()
    {
        popupMessageDict.Update();
    }

    public void PopupInputField(string defaultText, string directions, string confirmButtonText, string cancelButtonText, bool allowCopyToClipboard,
        Action<string> onSuccess, Action onFailure)
    {
        inputFieldDialog.Open(defaultText, directions, confirmButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure);
    }

    public void PopupActionSelection(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        actionSelectionPopup.Open(directions, cancelButtonText, onCancel, actions);
    }

    private void ShowMessage(string text)
    {
        TextMeshProUGUI spawned = Instantiate(textPrefab, popupMessagesList);
        spawned.text = text;
        spawnedMessagesDict.Add(text, spawned);
    }

    public void AddNewPopupMessage(string message, float duration = 3)
    {
        popupMessageDict.Add(message, duration);
    }

    private void RemoveMessage(string text)
    {
        TextMeshProUGUI retrievedText = spawnedMessagesDict[text];
        Destroy(retrievedText.gameObject);
        spawnedMessagesDict.Remove(text);
    }

    public void CopyTextToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
}