using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ActionSelection
{
    public string Text { get; private set; }
    public Action Action { get; private set; }
    public IEnumerator Coroutine { get; private set; }
    public bool HasCoroutine { get; private set; }

    public ActionSelection(string text, Action action) : this()
    {
        Text = text;
        Action = action;
        HasCoroutine = false;
    }

    public ActionSelection(string text, Action action, IEnumerator coroutine) : this()
    {
        Text = text;
        Action = action;
        Coroutine = coroutine;
        HasCoroutine = true;
    }

    public void AddAction(Action action)
    {
        Action += action;
    }
}

public class ActionSelectionPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI directionsText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;
    [SerializeField] private Transform actionList;
    [SerializeField] private ActionSelectionButton buttonPrefab;
    [SerializeField] private ActionSelectionButton cancelButton;
    [SerializeField] private GameObject display;

    private bool recievedResponse;
    private ActionSelection chosenSelection;

    public IEnumerator Consume(string directions, string cancelButtonText, Action onCancel, List<ActionSelection> actions)
    {
        // remove old options
        foreach (Transform child in actionList)
        {
            Destroy(child.gameObject);
        }

        // set text
        directionsText.text = directions;

        foreach (ActionSelection action in actions)
        {
            // create and set button
            ActionSelectionButton spawned = Instantiate(buttonPrefab, actionList);
            spawned.Set(new ActionSelection(action.Text, () =>
            {
                chosenSelection = action;
                recievedResponse = true;
            }));
        }

        // create cancel action selection
        ActionSelection cancelSelection = new ActionSelection(cancelButtonText, () =>
        {
            recievedResponse = true;
            onCancel?.Invoke();
        });
        cancelSelection.AddAction(() => chosenSelection = cancelSelection);

        // and add it to the cancel button
        cancelButton.Set(cancelSelection);

        yield return new WaitUntil(() => recievedResponse);
        recievedResponse = false;

        // Activate Callback
        chosenSelection.Action?.Invoke();

        display.SetActive(false);

        // Wait for Coroutine if there is one
        if (chosenSelection.HasCoroutine)
        {
            yield return StartCoroutine(chosenSelection.Coroutine);
        }

        Destroy(gameObject);
    }
}
