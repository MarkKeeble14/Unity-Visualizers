using UnityEngine;

public class EnableBasedOnActiveControlScheme : MonoBehaviour, IRecieveControlScheme
{
    [SerializeField] private GameObject controlActiveStateOf;
    [SerializeField] private ControlScheme activeOnThisScheme;
    public void RecieveControlScheme(ControlScheme controlScheme)
    {
        controlActiveStateOf.SetActive(controlScheme == activeOnThisScheme);
    }
}
