using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCameraToActiveCamera : MonoBehaviour, IRecieveActiveCamera
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private float planeDistance = 1;

    public void RecieveActiveCamera(Camera camera)
    {
        canvas.worldCamera = camera;
        canvas.planeDistance = planeDistance;
    }
}
