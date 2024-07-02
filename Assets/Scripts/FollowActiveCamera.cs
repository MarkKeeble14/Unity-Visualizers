using UnityEngine;

public class FollowActiveCamera : MonoBehaviour, IRecieveActiveCamera
{
    private Transform activeCameraTransform;

    private void Update()
    {
        if (activeCameraTransform == null) return;
        transform.position = activeCameraTransform.position;
    }

    public void RecieveActiveCamera(Camera camera)
    {
        activeCameraTransform = camera.transform;
    }
}
