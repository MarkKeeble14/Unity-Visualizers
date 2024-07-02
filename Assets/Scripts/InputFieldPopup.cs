using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

public class InputFieldPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI directionsText;
    [SerializeField] private TextMeshProUGUI acceptButtonText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private GameObject copyToClipboardButton;

    private bool recievedResponse;
    private bool accepted;

    private void OnEnable()
    {
        SetAcceptButtonInteractable(inputField.text);
    }

    public void SetAcceptButtonInteractable(string s)
    {
        acceptButton.interactable = (!string.IsNullOrEmpty(s));
    }

    public void Accept()
    {
        recievedResponse = true;
        accepted = true;
    }

    public void Cancel()
    {
        recievedResponse = true;
        accepted = false;
    }

    public void Open(string defaultText, string directions, string acceptButtonText, string cancelButtonText, bool allowCopyToClipboard, Action<string> onSuccess, Action onFailure)
    {
        gameObject.SetActive(true);
        StartCoroutine(Show(defaultText, directions, acceptButtonText, cancelButtonText, allowCopyToClipboard, onSuccess, onFailure));
    }

    private IEnumerator Show(string defaultText, string directions, string acceptButtonText, string cancelButtonText, bool allowCopyToClipboard, 
        Action<string> onSuccess, Action onFailure)
    {
        inputField.text = defaultText;
        directionsText.text = directions;
        this.acceptButtonText.text = acceptButtonText;
        this.cancelButtonText.text = cancelButtonText;

        copyToClipboardButton.SetActive(allowCopyToClipboard);

        yield return new WaitUntil(() => recievedResponse);
        recievedResponse = false;

        if (accepted)
        {
            onSuccess?.Invoke(inputField.text);
        }
        else
        {
            onFailure?.Invoke();
        }

        gameObject.SetActive(false);
    }

    public void CopyInputToKeyboard()
    {
        UIManager._Instance.CopyTextToClipboard(inputField.text);
    }
}
