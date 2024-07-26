using System;
using UnityEngine;

public class EnableBasedOnActiveControlScheme : MonoBehaviour, IRecieveControlScheme
{
    [SerializeField] private GameObject controlActiveStateOf;
    [SerializeField] private ControlScheme activeOnThisScheme;
    public Action OnEnabled;
    public Action OnDisabled;

    public void RecieveControlScheme(ControlScheme controlScheme)
    {
        controlActiveStateOf.SetActive(controlScheme == activeOnThisScheme);
        if (controlActiveStateOf.activeSelf)
        {
            OnEnabled?.Invoke();
        } else
        {
            OnDisabled?.Invoke();
        }
    }
}
