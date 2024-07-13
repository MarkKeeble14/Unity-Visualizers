using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class InputFieldPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI directionsText;
    [SerializeField] private TextMeshProUGUI acceptButtonText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private GameObject copyToClipboardButton;
    [SerializeField] private GameObject display;
    [SerializeField] private RectTransform textArea;

    private bool recievedResponse;
    private bool accepted;

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

    private void Update()
    {
        acceptButton.interactable = (!string.IsNullOrEmpty(inputField.text));
    }

    private void Init(string defaultText, string directions, string acceptButtonText, string cancelButtonText, bool allowCopyToClipboard)
    {
        inputField.text = defaultText;
        textArea.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        textArea.GetChild(2).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        inputField.Select();

        directionsText.text = directions;
        this.acceptButtonText.text = acceptButtonText;
        this.cancelButtonText.text = cancelButtonText;

        copyToClipboardButton.SetActive(allowCopyToClipboard);
    }

    private IEnumerator WaitForResponse()
    {
        yield return new WaitUntil(() => recievedResponse);
        recievedResponse = false;
    }

    public IEnumerator Consume(string defaultText, string directions, string acceptButtonText, string cancelButtonText, bool allowCopyToClipboard, 
        Action<string> onSuccess, Action onFailure)
    {
        Init(defaultText, directions, acceptButtonText, cancelButtonText, allowCopyToClipboard);

        yield return StartCoroutine(WaitForResponse());

        display.SetActive(false);

        if (accepted)
        {
            onSuccess?.Invoke(inputField.text);
        }
        else
        {
            onFailure?.Invoke();
        }

        Destroy(gameObject);
    }

    public IEnumerator Consume(string defaultText, string directions, string acceptButtonText, string cancelButtonText, bool allowCopyToClipboard,
    Func<string, IEnumerator> onSuccess, IEnumerator onFailure)
    {
        display.SetActive(true);

        Init(defaultText, directions, acceptButtonText, cancelButtonText, allowCopyToClipboard);

        yield return StartCoroutine(WaitForResponse());

        display.SetActive(false);

        if (accepted)
        {
            yield return StartCoroutine(onSuccess(inputField.text));
        }
        else
        {
            yield return StartCoroutine(onFailure);
        }

        Destroy(gameObject);
    }

    public void CopyInputToKeyboard()
    {
        UIManager._Instance.CopyTextToClipboard(inputField.text);
    }
}
