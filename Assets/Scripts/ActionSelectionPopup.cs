using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ActionSelection
{
    public string Text;
    public Action Action;

    public ActionSelection(string text, Action action) : this()
    {
        Text = text;
        Action = action;
    }
}

public class ActionSelectionPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI directionsText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;
    [SerializeField] private Transform actionList;
    [SerializeField] private ActionSelectionButton buttonPrefab;
    [SerializeField] private ActionSelectionButton cancelButton;

    private bool recievedResponse;

    public void Open(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        gameObject.SetActive(true);
        StartCoroutine(Show(directions, cancelButtonText, onCancel, actions));
    }

    private IEnumerator Show(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        foreach (Transform child in actionList)
        {
            Destroy(child.gameObject);
        }

        directionsText.text = directions;
        cancelButton.Set(new ActionSelection(cancelButtonText, onCancel));

        foreach (ActionSelection action in actions)
        {
            ActionSelectionButton spawned = Instantiate(buttonPrefab, actionList);
            spawned.Set(action);
            spawned.OnClick += () => recievedResponse = true;
        }

        yield return new WaitUntil(() => recievedResponse);
        recievedResponse = false;

        gameObject.SetActive(false);
    }
}
