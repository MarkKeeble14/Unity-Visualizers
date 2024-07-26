using UnityEngine;

public class SetRenderCameraToActiveCamera : MonoBehaviour, IRecieveActiveCamera
{
    [SerializeField] private Canvas canvas;

    public void RecieveCamera(Camera camera)
    {
        Debug.Log("Recieved Camera: " + camera);
        canvas.worldCamera = camera;
    }
}
