using UnityEngine;

public class InitializeEscapeMenuFunctions : MonoBehaviour
{
    [SerializeField] private EscapeMenuFunctions menuFunctions;

    private void Start()
    {
        menuFunctions.SetMusicMuted(false);
        menuFunctions.SetSFXMuted(false);
        menuFunctions.SetMusicVolume(1);
        menuFunctions.SetSFXVolume(1);
    }
}
