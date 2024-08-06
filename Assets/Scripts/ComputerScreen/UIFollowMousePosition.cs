using UnityEngine;

public class UIFollowMousePosition : MonoBehaviour
{
    [SerializeField] private RectTransform follower;
    [SerializeField] private RectTransform resultPlane;
    [SerializeField] private Transform raycastSourcePlane;
    [SerializeField] private Transform raycastTarget;
    [SerializeField] private Transform visual;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Transform maxScreenY;
    [SerializeField] private Transform maxScreenX;
    [SerializeField] private Transform minScreenY;
    [SerializeField] private Transform minScreenX;
    private Vector3 mousePos;
    private Vector3 raycastDirection;
    RaycastHit hit;

    private void Update()
    {
        // find point on screen that mouse is at
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, raycastSourcePlane.position.z * -1);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        raycastDirection = (raycastTarget.position - raycastSourcePlane.position).normalized;
        visual.position = mousePos;

        //Debug.DrawRay(mousePos, raycastDirection, Color.red, Mathf.Infinity);

        //Debug.Log("World Pos: <" + mousePos.x + ", " + mousePos.y + ">");

        //if (Physics.Raycast(mousePos, raycastDirection, out hit, Mathf.Infinity, hitMask))
        //{
        //    Debug.Log("Hit: " + hit.point);
            Vector2 screenViewPort = new Vector2(0.5f + (mousePos.x / (maxScreenX.position.x - minScreenX.position.x)),
                                0.5f - (mousePos.y / (maxScreenY.position.y - minScreenY.position.y)));

            //Debug.Log("Viewport: <" + screenViewPort.x + ", " + screenViewPort.y + ">");

            follower.anchoredPosition = new Vector2(resultPlane.sizeDelta.x * screenViewPort.x, resultPlane.sizeDelta.y * screenViewPort.y);
        //} else
        //{
        //    Debug.Log("No hit");
        //}
    }
}
