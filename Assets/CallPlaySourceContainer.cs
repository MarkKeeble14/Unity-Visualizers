using UnityEngine;

public class CallPlaySourceContainer : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private bool randomizeVolume;
    [SerializeField] private bool randomizePitch;

    public void PlaySource()
    {
        SFXManager._Instance.PlaySource(key, randomizeVolume, randomizePitch);
    }
}

