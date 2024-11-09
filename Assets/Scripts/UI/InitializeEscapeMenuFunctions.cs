using UnityEngine;

public class InitializeEscapeMenuFunctions : MonoBehaviour
{
    [SerializeField] private EscapeMenuFunctions menuFunctions;

    [SerializeField] private bool forceSetMusicVol;
    [SerializeField] private float forceSetMusicVolumeTo;
    [SerializeField] private bool forceSetSFXVol;
    [SerializeField] private float forceSetSFXVolumeTo;
    [SerializeField] private bool forceSetMusicMuted;
    [SerializeField] private bool forceSetMusicMutedTo;
    [SerializeField] private bool forceSetSFXMuted;
    [SerializeField] private bool forceSetSFXMutedTo;
    [SerializeField] private bool forceSetMasterVolume = true;
    [SerializeField] private float forceSetMasterVolumeTo = 1;

    private void Start()
    {
        if (forceSetMusicMuted)
        {
            menuFunctions.SetMusicMuted(forceSetMusicMutedTo);
        }
        else
        {
            // Music Muted
            if (PlayerPrefs.HasKey(EscapeMenuFunctions.MusicMutedPrefsKey))
            {
                menuFunctions.SetMusicMuted(PlayerPrefs.GetString(EscapeMenuFunctions.MusicMutedPrefsKey).ToUpper() == "TRUE");
            }
            else
            {
                menuFunctions.SetMusicMuted(false);
            }
        }

        if (forceSetSFXMuted)
        {
            menuFunctions.SetMusicMuted(forceSetSFXMutedTo);
        }
        else
        {
            // SFX Muted
            if (PlayerPrefs.HasKey(EscapeMenuFunctions.SFXMutedPrefsKey))
            {
                menuFunctions.SetSFXMuted(PlayerPrefs.GetString(EscapeMenuFunctions.SFXMutedPrefsKey).ToUpper() == "TRUE");
            }
            else
            {
                menuFunctions.SetSFXMuted(false);
            }
        }

        if (forceSetMusicVol)
        {
            menuFunctions.OnlySetMusicVolume(forceSetMusicVolumeTo);
        } else
        {
            // Music Volume
            if (PlayerPrefs.HasKey(EscapeMenuFunctions.MusicVolumePrefsKey))
            {
                menuFunctions.SetMusicVolume(PlayerPrefs.GetFloat(EscapeMenuFunctions.MusicVolumePrefsKey));
            }
            else
            {
                menuFunctions.SetMusicVolume(1);
            }
        }

        if (forceSetSFXVol)
        {
            menuFunctions.OnlySetSFXVolume(forceSetSFXVolumeTo);
        }
        else
        {
            // SFX Volume
            if (PlayerPrefs.HasKey(EscapeMenuFunctions.SFXVolumePrefsKey))
            {
                menuFunctions.SetSFXVolume(PlayerPrefs.GetFloat(EscapeMenuFunctions.SFXVolumePrefsKey));
            }
            else
            {
                menuFunctions.SetSFXVolume(1);
            }
        }

        if (forceSetMasterVolume)
        {
            menuFunctions.SetMasterVolume(forceSetMasterVolumeTo);
        }
    }
}
