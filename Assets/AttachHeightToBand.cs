using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AttachHeightToBand : MonoBehaviour
{
    [SerializeField, Range(0, 7)] private int band;
    [SerializeField] private float multiplier = 25;
    [SerializeField] private float defaultHeight = 0;
    [SerializeField] private float adjustSpeed = 25;

    public bool SlowAdjust { get; set; }

    private float targetValue;
    private float currentValue;
    private RectTransform rectTransform;

    public void Set(int band, float multiplier, float defaultHeight, float adjustSpeed)
    {
        this.band = band;
        Set(multiplier, defaultHeight, adjustSpeed);
    }

    public void Set(float multiplier, float defaultHeight, float adjustSpeed)
    {
        this.multiplier = multiplier;
        this.defaultHeight = defaultHeight;
        this.adjustSpeed = adjustSpeed;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SlowAdjust)
        {
            // Calculate Adjustment
            targetValue = defaultHeight + VisualizerManager._Instance.GetBandValue(band, false) * multiplier;
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * adjustSpeed);

            // Adjust
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, currentValue);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultHeight + VisualizerManager._Instance.GetBandValue(band, true) * multiplier);
        }
    }
}
