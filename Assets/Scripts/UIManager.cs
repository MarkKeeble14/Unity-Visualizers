using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }

    [SerializeField] private Transform uiStack;

    [Header("Messages")]
    [SerializeField] private Transform popupMessagesList;
    [SerializeField] private PopupMessage messagePopupPrefab;
    [SerializeField] private List<SerializableKeyValuePair<MessageClass, Color>> messageClassColors = new();
    private TimerDictionary<string> popupMessageDict = new();
    private Dictionary<string, PopupMessage> spawnedMessagesDict = new();
    private List<KeyValuePair<string, MessageClass>> addedMessages = new();

    [Header("Loading")]
    [SerializeField] private Transform popupLoadingList;
    [SerializeField] private PopupLoading loadingPopupPrefab;
    private Dictionary<int, PopupLoading> spawnedLoadingDict = new();

    [Header("References")]
    [SerializeField] private InputFieldPopup inputFieldDialogPrefab;
    [SerializeField] private ActionSelectionPopup actionSelectionPopupPrefab;

    public bool IsPopupOpen => uiStack.childCount > 0;

    public enum MessageClass
    {
        INFO,
        WARNING,
        ERROR,
        SUCCESS
    }


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

    public IEnumerator PopupInputField(string defaultText, string directions, string confirmButtonText, string cancelButtonText, bool allowCopyToClipboard,
        Action<string> onSuccess, Action onFailure)
    {
        KeyControl.Disable = true;

        InputFieldPopup inputFieldPopup = Instantiate(inputFieldDialogPrefab, uiStack);
        yield return inputFieldPopup.Consume(defaultText, directions, confirmButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure);

        KeyControl.Disable = false;
    }

    public IEnumerator PopupInputField(string defaultText, string directions, string confirmButtonText, string cancelButtonText, bool allowCopyToClipboard,
    Func<string, IEnumerator> onSuccess,IEnumerator onFailure)
    {
        KeyControl.Disable = true;

        InputFieldPopup inputFieldPopup = Instantiate(inputFieldDialogPrefab, uiStack);
        yield return inputFieldPopup.Consume(defaultText, directions, confirmButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure);

        KeyControl.Disable = false;
    }

    public IEnumerator PopupActionSelection(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        ActionSelectionPopup actionSelectionPopup = Instantiate(actionSelectionPopupPrefab, uiStack);
        yield return actionSelectionPopup.Consume(directions, cancelButtonText, onCancel, actions);
    }

    public int AddLoading(string message)
    {
        // spawn the prefab
        PopupLoading spawned = Instantiate(loadingPopupPrefab, popupLoadingList);
        spawned.Set(message);

        // find a key
        int messageKey = 0;
        while (spawnedLoadingDict.ContainsKey(messageKey)) { messageKey++; }

        spawnedLoadingDict.Add(messageKey, spawned);

        return messageKey;
    }

    public void RemoveLoading(int key)
    {
        PopupLoading loading = spawnedLoadingDict[key];
        spawnedLoadingDict.Remove(key);
        Destroy(loading.gameObject);
        return;
    }

    public void AddNewMessage(MessageClass messageClass, string message, float duration = 3)
    {
        addedMessages.Add(new KeyValuePair<string, MessageClass>(message, messageClass));
        popupMessageDict.Add(message, duration);
    }

    private Color GetMessageClassColor(MessageClass messageClass)
    {
        foreach (SerializableKeyValuePair<MessageClass, Color> kvp in messageClassColors)
        {
            if (kvp.Key == messageClass) return kvp.Value;
        }
        return messageClassColors[0].Value;
    }

    private void ShowMessage(string text)
    {
        PopupMessage spawned = Instantiate(messagePopupPrefab, popupMessagesList);

        for (int i = 0; i < addedMessages.Count; i++)
        {
            if (addedMessages[i].Key.Equals(text))
            {
                spawned.SetColor(GetMessageClassColor(addedMessages[i].Value));
                addedMessages.RemoveAt(i);
            }
        }

        spawned.SetText(text);
        spawnedMessagesDict.Add(text, spawned);
    }

    private void RemoveMessage(string text)
    {
        PopupMessage retrievedText = spawnedMessagesDict[text];
        Destroy(retrievedText.gameObject);
        spawnedMessagesDict.Remove(text);
    }

    public void CopyTextToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
}