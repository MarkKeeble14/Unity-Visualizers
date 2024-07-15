using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTrackTitleCaller : MonoBehaviour
{
    public void EditTrackTitle()
    {
        StartCoroutine(UIManager._Instance.PopupInputField(VisualizerManager._Instance.TrackTitle, "Enter a new Title", "Accept", "Cancel", false, 
            x => VisualizerManager._Instance.UpdateTrackTitle(x), null));
    }
}
