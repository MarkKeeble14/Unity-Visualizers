using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMousePositionWorld : MonoBehaviour
{
    [SerializeField] private float zPlane;
    private Vector3 mousePos;
    private Vector3 inputPos;

    // Update is called once per frame
    void Update()
    {
        inputPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1);
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);
        mousePos.z = zPlane;
        transform.position = mousePos;
    }
}
