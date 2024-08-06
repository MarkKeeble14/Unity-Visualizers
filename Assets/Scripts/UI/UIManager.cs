using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public struct StringWithAnIndex
{
    public int index;
    public string str;

    public StringWithAnIndex(int index, string str)
    {
        this.index = index;
        this.str = str;
    }
}

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }

    [SerializeField] private Transform uiStack;

    [Header("Messages")]
    [SerializeField] private Transform popupMessagesList;
    [SerializeField] private Transform[] mirroredPopupMessageLists;
    private Dictionary<Transform, List<GameObject>> mirroredPopupMessages = new();

    [SerializeField] private PopupMessage messagePopupPrefab;
    [SerializeField] private List<SerializableKeyValuePair<MessageClass, Color>> messageClassColors = new();
    private TimerDictionary<StringWithAnIndex> popupMessageDict = new();
    private List<KeyValuePair<string, PopupMessage>> spawnedMessagesDict = new();
    private List<KeyValuePair<string, MessageClass>> addedMessages = new();

    [Header("Loading")]
    [SerializeField] private Transform popupLoadingList;
    [SerializeField] private PopupLoading loadingPopupPrefab;
    private Dictionary<int, PopupLoading> spawnedLoadingDict = new();

    [Header("References")]
    [SerializeField] private InputFieldPopup inputFieldDialogPrefab;
    [SerializeField] private ActionSelectionPopup actionSelectionPopupPrefab;
    private Dictionary<PopupLoading, float> minTimeDurationDict = new();
    private List<PopupLoading> loadingPopupsCanDestroy = new();
    private Queue<PopupLoading> loadingPopupsToDestroy = new();

    private int numMessagesLifetime;

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

        UpdateLoadingPopups();
    }

    private void UpdateLoadingPopups()
    {
        for (int i = 0; i < loadingPopupsCanDestroy.Count; i++)
        {
            if (loadingPopupsCanDestroy[i].TimeAlive > minTimeDurationDict[loadingPopupsCanDestroy[i]])
            {
                loadingPopupsToDestroy.Enqueue(loadingPopupsCanDestroy[i]);
            }
        }

        while (loadingPopupsToDestroy.Count > 0)
        {
            PopupLoading destroying = loadingPopupsToDestroy.Dequeue();
            loadingPopupsCanDestroy.Remove(destroying);
            minTimeDurationDict.Remove(destroying);
            Destroy(destroying.gameObject);
        }
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

    public int AddLoading(string message, float minDuration = 0)
    {
        // spawn the prefab
        PopupLoading spawned = Instantiate(loadingPopupPrefab, popupLoadingList);
        spawned.Set(message);


        // find a key
        int messageKey = 0;
        while (spawnedLoadingDict.ContainsKey(messageKey)) { messageKey++; }
        spawnedLoadingDict.Add(messageKey, spawned);

        minTimeDurationDict.Add(spawned, minDuration);

        return messageKey;
    }

    public void RemoveLoading(int key)
    {
        PopupLoading loading = spawnedLoadingDict[key];
        spawnedLoadingDict.Remove(key);
        loadingPopupsCanDestroy.Add(loading);

        return;
    }

    public void AddNewMessage(MessageClass messageClass, string message, float duration = 5)
    {
        addedMessages.Add(new KeyValuePair<string, MessageClass>(message, messageClass));
        popupMessageDict.Add(new StringWithAnIndex(numMessagesLifetime++, message), duration);
    }

    private Color GetMessageClassColor(MessageClass messageClass)
    {
        foreach (SerializableKeyValuePair<MessageClass, Color> kvp in messageClassColors)
        {
            if (kvp.Key == messageClass) return kvp.Value;
        }
        return messageClassColors[0].Value;
    }

    private void ShowMessage(StringWithAnIndex str)
    {
        PopupMessage spawned = Instantiate(messagePopupPrefab, popupMessagesList);

        for (int i = 0; i < addedMessages.Count; i++)
        {
            if (addedMessages[i].Key.Equals(str.str))
            {
                spawned.SetColor(GetMessageClassColor(addedMessages[i].Value));
                addedMessages.RemoveAt(i);
            }
        }

        // 
        spawned.SetText(str.str);
        spawnedMessagesDict.Add(new KeyValuePair<string, PopupMessage>(str.str, spawned));

        // spawn mirror messages
        List<GameObject> mirroredMessages = new();
        foreach (Transform t in mirroredPopupMessageLists)
        {
            PopupMessage copy = Instantiate(messagePopupPrefab, t);
            copy.SetText(spawned.Text);
            copy.SetColor(spawned.Color);
            mirroredMessages.Add(copy.gameObject);
        }
        mirroredPopupMessages.Add(spawned.transform, mirroredMessages);
    }

    private void RemoveMessage(StringWithAnIndex str)
    {
        PopupMessage retrievedText = null;
        for (int i = 0; i < spawnedMessagesDict.Count; i++)
        {
            KeyValuePair<string, PopupMessage> kvp = spawnedMessagesDict[i];
            if (kvp.Key == str.str)
            {
                retrievedText = kvp.Value;
                spawnedMessagesDict.RemoveAt(i);
                break;
            }
        }

        if (retrievedText == null) { throw new ElementWithKeyNotFoundException(typeof(PopupMessage), str.str); }

        // destroy mirrored messages
        List<GameObject> retrievedMirroredMessages = mirroredPopupMessages[retrievedText.transform];
        foreach (GameObject obj in retrievedMirroredMessages) { Destroy(obj); }
        mirroredPopupMessages.Remove(retrievedText.transform);

        Destroy(retrievedText.gameObject);
    }

    public void CopyTextToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
}