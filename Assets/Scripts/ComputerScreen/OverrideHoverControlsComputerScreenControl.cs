using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverrideHoverControlsComputerScreenControl : ComputerScreenControl
{
    [SerializeField] private OnHoverHandler[] hoverHandlers;

    protected override void LoadEvents()
    {
        whileHovered += () =>
        {
            foreach (OnHoverHandler handler in hoverHandlers)
            {
                handler.OverrideControl = true;
                handler.Hovered();
            }
        };

        whileNotHovered += () =>
        {
            foreach (OnHoverHandler handler in hoverHandlers)
            {
                handler.OverrideControl = false;
            }
        };
    }
}
