using UnityEngine;

public abstract class AddToOnSongEnd : MonoBehaviour
{
    public abstract void CallOnSongEnd();

    private void Start()
    {
        VisualizerManager._Instance.OnSongEnd += CallOnSongEnd;
    }
}
