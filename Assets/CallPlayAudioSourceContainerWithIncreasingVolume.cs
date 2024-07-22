using UnityEngine;

public class CallPlayAudioSourceContainerWithIncreasingVolume : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private float startVol;
    [SerializeField] private float goalVol;
    [SerializeField] private float rateOfChange;
    [SerializeField] private MathHelper.AlterationMethod method;

    public void PlaySource()
    {
        SFXManager._Instance.PlaySourceWithIncreasingVolume(key, startVol, goalVol, rateOfChange, method);
    }
}

