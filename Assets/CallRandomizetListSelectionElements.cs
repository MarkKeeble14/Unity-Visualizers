using System.Collections.Generic;
using UnityEngine;

public class CallRandomizetListSelectionElements : MonoBehaviour
{
    [SerializeField] private Transform listSelectionElementsHolder;

    public void RandomizeElements()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Confirm Randomization", "Cancel", null,
            new List<ActionSelection>()
            {
                new ActionSelection("Confirm", () =>
                {
                    foreach (Transform t in listSelectionElementsHolder)
                    {
                        ListSelectionElement element = t.GetComponent<ListSelectionElement>();
                        element.Randomize();
                    }
                })
            }));
    }
}
