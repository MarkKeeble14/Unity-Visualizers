using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetFillToTrackPlaythroughPercent : MonoBehaviour
{
    [SerializeField] private float fillSpeed;
    [SerializeField] private MathHelper.AlterationMethod method;
    private float currentValue;

    private Image toFill;

    private void Awake()
    {
        toFill = GetComponent<Image>();
    }

    private void Update()
    {
        currentValue = MathHelper.GetNextValue(currentValue, VisualizerManager._Instance.PlaythroughPercent, fillSpeed, method, true);
        toFill.fillAmount = currentValue;
    }
}