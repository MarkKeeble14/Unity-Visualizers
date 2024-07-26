using UnityEngine;

public class EnableCameraBasedOnActiveControlScheme : EnableBasedOnActiveControlScheme
{
    [SerializeField] private Camera camera;
    private void Awake()
    {
        OnEnabled += () =>
        {
            if (VisualizerManager._Instance != null) { VisualizerManager._Instance.SetActiveCamera(camera); }
        };
    }
}