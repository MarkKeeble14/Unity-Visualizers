using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(DropShadow))]
public class DropShadowMatchImageColor : MonoBehaviour
{
    [SerializeField] private DropShadow[] dropShadows;
    [SerializeField] private Image image;

    private void Update()
    {
        foreach (DropShadow dShadow in dropShadows)
        {
            dShadow.effectColor = image.color;
        }
    }
}
