using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControlSchemes : ActOnKeyPress
{
    [SerializeField] private GameObject[] controlSchemes;
    private int currentlyActiveSchemeIndex;

    private void Start()
    {
        DisableAllExceptActive();
    }

    private void DisableAllExceptActive()
    {
        for (int i = 0; i <  controlSchemes.Length; ++i) 
            controlSchemes[i].SetActive(i == currentlyActiveSchemeIndex);
    }

    protected override void Act()
    {
        ++currentlyActiveSchemeIndex;
        if (currentlyActiveSchemeIndex >= controlSchemes.Length)
            currentlyActiveSchemeIndex = 0;
        DisableAllExceptActive();
    }
}
