using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PostProcessingCamera : MonoBehaviour
{
    [SerializeField] private UniversalAdditionalCameraData cameraData;
    public UniversalAdditionalCameraData CameraData => cameraData;
}
