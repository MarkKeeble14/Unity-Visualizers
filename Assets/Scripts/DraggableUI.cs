using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform transformOfUIToMove;
    [SerializeField] private RectTransform canvasRect;
    private Vector2 dragOffset;

    [SerializeField] private bool forceLockOnScreen = true;

    public void BeginDrag(Vector2 pos)
    {
        dragOffset = pos - new Vector2(transformOfUIToMove.position.x, transformOfUIToMove.position.y);
    }

    public void Drag(Vector2 pos)
    {
        transformOfUIToMove.position = pos - dragOffset;
    }

    public void EndDrag()
    {
        if (forceLockOnScreen)
            KeepFullyOnScreen(transformOfUIToMove, canvasRect);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDrag(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
    }

    private void KeepFullyOnScreen(RectTransform rect, RectTransform canvas)
    {
        Vector2 anchorOffset = canvas.sizeDelta * (rect.anchorMin - Vector2.one / 2);

        Vector2 maxPivotOffset = rect.sizeDelta * (rect.pivot - (Vector2.one / 2) * 2);
        Vector2 minPivotOffset = rect.sizeDelta * ((Vector2.one / 2) * 2 - rect.pivot);

        Vector2 position = rect.anchoredPosition;

        float minX = (canvas.sizeDelta.x) * -0.5f - anchorOffset.x - minPivotOffset.x + rect.sizeDelta.x;
        float maxX = (canvas.sizeDelta.x) * 0.5f - anchorOffset.x + maxPivotOffset.x;
        float minY = (canvas.sizeDelta.y) * -0.5f - anchorOffset.y - minPivotOffset.y + rect.sizeDelta.y;
        float maxY = (canvas.sizeDelta.y) * 0.5f - anchorOffset.y + maxPivotOffset.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        rect.anchoredPosition = position;
    }
}
