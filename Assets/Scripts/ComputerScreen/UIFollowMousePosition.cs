using UnityEngine;

public class UIFollowMousePosition : MonoBehaviour
{
    [SerializeField] private RectTransform follower;
    [SerializeField] private RectTransform resultPlane;
    [SerializeField] private Transform raycastSourcePlane;
    [SerializeField] private Transform raycastTarget;
    [SerializeField] private Transform visual;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Transform minScreenY;
    [SerializeField] private Transform maxScreenY;
    [SerializeField] private Transform minScreenX;
    [SerializeField] private Transform maxScreenX;
    private Vector3 mousePos;

    private void Update()
    {
        // find point on screen that mouse is at
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, raycastSourcePlane.position.z * -1);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        visual.position = mousePos;

        Vector2 screenViewPort = new Vector2(0.5f + (mousePos.x / (maxScreenX.position.x - minScreenX.position.x)),
                            0.5f - (mousePos.y / (minScreenY.position.y - maxScreenY.position.y)));

        follower.anchoredPosition = new Vector2(resultPlane.sizeDelta.x * screenViewPort.x, resultPlane.sizeDelta.y * screenViewPort.y);
    }
}
