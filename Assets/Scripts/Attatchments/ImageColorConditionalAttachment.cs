using UnityEngine;
using UnityEngine.UI;

public class ImageColorConditionalAttachment : ConditionalExecutionAttachParameter
{
    [SerializeField] private Image image;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;

    protected override void SetParameter(float value)
    {
        //
    }

    protected override void SetParameterCalledWhileLocked(float value)
    {
        image.color = lockedColor;
    }

    protected override void SetParameterCalledWhileNotLocked(float value)
    {
        image.color = unlockedColor;
    }
}