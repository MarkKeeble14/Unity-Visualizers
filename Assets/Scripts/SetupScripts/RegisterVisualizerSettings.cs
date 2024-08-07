using UnityEngine;

public class RegisterVisualizerSettings : MonoBehaviour
{
    [SerializeField] private AddSetting[] settings;
    [SerializeField] private bool autoFetchFromGameObject = true;

    private void Start()
    {
        if (autoFetchFromGameObject) { settings = GetComponents<AddSetting>(); }
        foreach (AddSetting setting in settings) { setting.MakeSetting(); }
    }
}
