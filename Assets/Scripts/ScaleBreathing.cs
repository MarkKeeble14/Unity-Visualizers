using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBreathing : MonoBehaviour
{
    [SerializeField] private Axis scaleOn;
    [SerializeField] private float minValue = -.5f;
    [SerializeField] private float maxValue = .5f;
    [SerializeField] private float breatheSpeed = 1;
    [SerializeField] private float currentValue = 1;
    private float targetValue;

    private void Start()
    {
        targetValue = RandomHelper.RandomBool() ? minValue : maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentValue >= maxValue)
            targetValue = minValue;
        else if (currentValue <= minValue)
            targetValue = maxValue;

        currentValue = Mathf.MoveTowards(currentValue, targetValue, Time.deltaTime * breatheSpeed);

        switch (scaleOn)
        {
            case Axis.X:
                transform.localScale = new Vector3(currentValue, transform.localScale.y, transform.localScale.z);
                break;
            case Axis.Y:
                transform.localScale = new Vector3(transform.localScale.x, currentValue, transform.localScale.z);
                break;
            case Axis.Z:
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, currentValue);
                break;
        }
    }
}
