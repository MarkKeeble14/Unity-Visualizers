using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SongPositionSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private float fillSpeed;
    [SerializeField] private MathHelper.AlterationMethod method;
    private float currentValue;

    private bool moving;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetMoving(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetMoving(false);
    }

    public void SetMoving(bool b)
    {
        moving = b;
    }

    public void SetTrackPosition(float v)
    {
        VisualizerManager._Instance.SetPlaythroughPosition(v);
    }

    private void Update()
    {
        if (moving)
        {
            currentValue = slider.value;
            SetTrackPosition(currentValue);
            return;
        }
        currentValue = MathHelper.GetNextValue(currentValue, VisualizerManager._Instance.PlaythroughPercent, fillSpeed, method, true);
        slider.value = currentValue;
    }
}

