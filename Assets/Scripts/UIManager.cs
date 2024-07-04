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
    private TimerDictionary<string> popupMessageDict = new();
    private Dictionary<string, PopupMessage> spawnedMessagesDict = new();

    [Header("Loading")]
    [SerializeField] private Transform popupLoadingList;
    [SerializeField] private PopupLoading loadingPopupPrefab;
    private List<PopupLoading> spawnedLoadingDict = new();

    [Header("References")]
    [SerializeField] private InputFieldPopup inputFieldDialogPrefab;
    [SerializeField] private ActionSelectionPopup actionSelectionPopupPrefab;

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
        InputFieldPopup inputFieldPopup = Instantiate(inputFieldDialogPrefab, uiStack);
        yield return inputFieldPopup.Show(defaultText, directions, confirmButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure);
    }

    public IEnumerator PopupInputField(string defaultText, string directions, string confirmButtonText, string cancelButtonText, bool allowCopyToClipboard,
    Func<string, IEnumerator> onSuccess,IEnumerator onFailure)
    {
        InputFieldPopup inputFieldPopup = Instantiate(inputFieldDialogPrefab, uiStack);
        yield return inputFieldPopup.Show(defaultText, directions, confirmButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure);
    }

    public IEnumerator PopupActionSelection(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        ActionSelectionPopup actionSelectionPopup = Instantiate(actionSelectionPopupPrefab, uiStack);
        yield return actionSelectionPopup.Show(directions, cancelButtonText, onCancel, actions);
    }

    public void AddLoading(string message)
    {
        PopupLoading spawned = Instantiate(loadingPopupPrefab, popupLoadingList);
        spawned.Set(message);
        spawnedLoadingDict.Add(spawned);
    }

    public void RemoveLoading(string text)
    {
        foreach (PopupLoading pop in spawnedLoadingDict)
        {
            if (pop.Text.Equals(text))
            {
                spawnedLoadingDict.Remove(pop);
                Destroy(pop.gameObject);
                return;
            }
        }
    }

    public void AddNewMessage(string message, float duration = 3)
    {
        popupMessageDict.Add(message, duration);
    }

    private void ShowMessage(string text)
    {
        PopupMessage spawned = Instantiate(messagePopupPrefab, popupMessagesList);
        spawned.Set(text);
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