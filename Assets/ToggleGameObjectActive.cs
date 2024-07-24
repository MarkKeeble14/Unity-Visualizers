using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObjectActive : MonoBehaviour
{
    [SerializeField] private GameObject obj;

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }
}
