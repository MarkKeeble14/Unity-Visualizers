using UnityEngine;

public abstract class TweenOnHover : MonoBehaviour
{
    [SerializeField] protected float tweenSpeed;
    [SerializeField] protected MathHelper.AlterationMethod tweenMethod;

    private void Update()
    {
        if (UIHelper.IsPointerOverSpecificUIElement(gameObject))
        {
            Hovered();
        }
        else
        {
            NotHovered();
        }
        Tween();
    }

    protected abstract void Tween();

    protected abstract void Hovered();

    protected abstract void NotHovered();
}
