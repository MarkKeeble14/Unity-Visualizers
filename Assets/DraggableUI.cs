using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform transformOfUIToMove;
    private Vector2 dragOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = eventData.position - new Vector2(transformOfUIToMove.position.x, transformOfUIToMove.position.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transformOfUIToMove.position = eventData.position - dragOffset;
    }
}
