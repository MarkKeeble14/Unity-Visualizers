using System.Collections.Generic;
using UnityEngine;

public class CallRandomizetListSelectionElements : MonoBehaviour
{
    [SerializeField] private Transform listSelectionElementsHolder;
    [SerializeField] private string listLabel;

    public void RandomizeElements()
    {
        UIManager._Instance.PopupAreYouSureMessage("Randomize ALL " + listLabel + "?",
            () =>
            {
                foreach (Transform t in listSelectionElementsHolder)
                {
                    ListSelectionElement element = t.GetComponent<ListSelectionElement>();
                    element.ForceRandomize();
                }
            }, null);
    }
}
