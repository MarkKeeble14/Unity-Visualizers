using UnityEngine;

public class SetRenderCameraToActiveCamera : MonoBehaviour, IRecieveActiveCamera
{
    [SerializeField] private Canvas canvas;

    public void RecieveCamera(Camera camera)
    {
        canvas.worldCamera = camera;
    }
}
