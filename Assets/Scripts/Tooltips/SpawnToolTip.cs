using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform spawnOn;
    [SerializeField] private string toolTipText;
    private ToolTip spawnedToolTip;

    public void CreateToolTip()
    {
        if (spawnedToolTip != null) return;
        spawnedToolTip = ToolTipManager._Instance.RequestToolTip(spawnOn, container, toolTipText);
    }

    public void DestroyToolTip()
    {
        if (spawnedToolTip == null) return;
        Destroy(spawnedToolTip.gameObject);
    }

    public void SetToolTipText(string text) => toolTipText = text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CreateToolTip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyToolTip();
    }
}
