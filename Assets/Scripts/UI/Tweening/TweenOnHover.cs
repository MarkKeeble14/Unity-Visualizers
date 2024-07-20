using UnityEngine;

public abstract class TweenOnHover : MonoBehaviour
{
    [SerializeField] protected float tweenSpeed;
    [SerializeField] protected MathHelper.AlterationMethod tweenMethod;
    public bool OverrideControl;

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
        Tween();
    }

    protected abstract void Tween();

    public abstract void Hovered();

    public abstract void NotHovered();
}
