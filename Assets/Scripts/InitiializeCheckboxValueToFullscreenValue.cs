using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiializeCheckboxValueToFullscreenValue : MonoBehaviour
{
    [SerializeField] private Checkbox checkbox;
    
    private void Start()
    {
        checkbox.Active = Screen.fullScreen;
    }
}
