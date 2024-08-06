using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedUIFollowMousePosition : MonoBehaviour
{
    [SerializeField] private RectTransform follower;
    [SerializeField] private Vector2 boundingBox;
    [SerializeField] private Transform plane;
    private Vector3 screenToViewportPoint;
    private Vector2 positionWithinBoundingBox;

    private void Update()
    {
        screenToViewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        positionWithinBoundingBox = new Vector2(screenToViewportPoint.x * boundingBox.x, 
                                                screenToViewportPoint.y * boundingBox.y);
        follower.anchoredPosition = new Vector3(    positionWithinBoundingBox.x,
                                                    positionWithinBoundingBox.y,
                                                    plane.position.z);
    }
}
