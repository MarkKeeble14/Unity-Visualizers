using System.Collections.Generic;
using UnityEngine;

public class CallRandomizetListSelectionElements : MonoBehaviour
{
    [SerializeField] private Transform listSelectionElementsHolder;
    [SerializeField] private string listLabel;

    public void RandomizeElements()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Confirm randomization of " + listLabel, "Cancel", null,
            new List<ActionSelection>()
            {
                new ActionSelection("Confirm", () =>
                {
                    foreach (Transform t in listSelectionElementsHolder)
                    {
                        ListSelectionElement element = t.GetComponent<ListSelectionElement>();
                        element.TryRandomize();
                    }
                })
            }));
    }
}
