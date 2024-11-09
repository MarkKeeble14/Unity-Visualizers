using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallRandomizeVisualizerElements : MonoBehaviour
{
    [SerializeField] private Transform visualizerElementsHolder;

    public void RandomizeVisualizerElements()
    {
        UIManager._Instance.PopupAreYouSureMessage("Randomize ALL Visualizer Elements?",
            () =>
            {
                foreach (Transform t in visualizerElementsHolder)
                {
                    VisualizerSetupElement element = t.GetComponent<VisualizerSetupElement>();
                    element.TryRandomize();
                }
            }, null);
    }
}
