using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallRandomizeVisualizerElements : MonoBehaviour
{
    [SerializeField] private Transform visualizerElementsHolder;

    public void RandomizeVisualizerElements()
    {
        StartCoroutine(UIManager._Instance.PopupActionSelection("Confirm randomization of elements", "Cancel", null,
            new List<ActionSelection>()
            {
                new ActionSelection("Confirm", () =>
                {
                    foreach (Transform t in visualizerElementsHolder)
                    {
                        VisualizerSetupElement element = t.GetComponent<VisualizerSetupElement>();
                        element.TryRandomize();
                    }
                })
            }));
    }
}
