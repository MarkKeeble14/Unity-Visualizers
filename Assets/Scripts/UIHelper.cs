using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHelper
{
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    public static bool IsPointerOverSpecificUIElement(GameObject specificGameObject)
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults(), specificGameObject);
    }

    // Returns 'true' if we touched or hovering on Unity UI element.
    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject specificGameObject = null)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == 5) // 5 is the UI layer
            {
                if (specificGameObject)
                {
                    if (curRaysastResult.gameObject == specificGameObject) return true;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }


    // Gets all event system raycast results of current mouse or touch position.
    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
