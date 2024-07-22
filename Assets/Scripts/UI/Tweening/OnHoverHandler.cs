using System;
using UnityEngine;

public abstract class OnHoverHandler : MonoBehaviour
{
    public bool OverrideControl;

    public abstract void Hovered();

    public abstract void NotHovered();

    protected Action onUpdate;

    private void Update()
    {
        if (!OverrideControl)
        {
            if (UIHelper.IsPointerOverSpecificUIElement(gameObject))
            {
                Hovered();
            }
            else
            {
                NotHovered();
            }
        }
        onUpdate?.Invoke();
    }
}
