using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderComputerControl : ComputerScreenControl
{
    [SerializeField] private float scalingFactor = 1;
    [SerializeField] private Slider slider;
    private Vector2 curPos;
    private Vector2 lastPos;
    private Vector2 movement;

    protected override void LoadEvents()
    {
        clicked += () =>
        {
            lastPos = Input.mousePosition;
        };

        held += () =>
        {
            curPos = Input.mousePosition;
            movement = curPos - lastPos;

            slider.value += movement.x * scalingFactor;

            lastPos = curPos;
        };
    }
}
