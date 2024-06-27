using System.Collections;
using UnityEngine;

public abstract class AlterFloatGameEvent : GameEvent 
{
    [SerializeField] private bool instantChange;
    [SerializeField] private MathHelper.AlterationMethod alterationMethod;
    [SerializeField] private float changeSpeed;
    [SerializeField] private float changeTo;

    protected abstract void SetValue(float newValue);

    protected abstract float GetValue();

    public override void Activate()
    {
        if (instantChange)
        {
            SetValue(changeTo);
        } else
        {
            StartCoroutine(ChangeValue());
        }
    }

    private IEnumerator ChangeValue()
    {
        if (changeTo > GetValue())
        {
            while (GetValue() < changeTo)
            {
                SetValue(MathHelper.GetNextValue(GetValue(), changeTo, changeSpeed, alterationMethod, true));

                yield return null;
            }
        } else if (changeTo < GetValue())
        {
            while (GetValue() > changeTo)
            {
                SetValue(MathHelper.GetNextValue(GetValue(), changeTo, changeSpeed, alterationMethod, true));

                yield return null;
            }
        }
    }
}
