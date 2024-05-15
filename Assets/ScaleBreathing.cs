using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBreathing : MonoBehaviour
{
    [SerializeField] private Vector3 axisMultipliers = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 currentAxisValues = new Vector3(0, 0, 0);
    [SerializeField] private float minValue = -.5f;
    [SerializeField] private float maxValue = .5f;
    [SerializeField] private float breatheSpeed = 1;
    private float targetValue;

    private void Start()
    {
        targetValue = Random.Range(0, 100) > 50 ? minValue : maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetValue >= maxValue)
            targetValue = minValue;
        else if (targetValue <= minValue)
            targetValue = maxValue;

        // x
        if (axisMultipliers.x == 0)
        {
            currentAxisValues.x = 1;
        }
        else
        {
            currentAxisValues.x = Mathf.Lerp(currentAxisValues.x, targetValue, Time.deltaTime * breatheSpeed * axisMultipliers.x);
        }

        // y
        if (axisMultipliers.y == 0)
        {
            currentAxisValues.y = 1;
        }
        else
        {
            currentAxisValues.y = Mathf.Lerp(currentAxisValues.y, targetValue, Time.deltaTime * breatheSpeed * axisMultipliers.y);
        }

        // z
        if (axisMultipliers.z == 0)
        {
            currentAxisValues.z = 1;
        }
        else
        {
            currentAxisValues.z = Mathf.Lerp(currentAxisValues.z, targetValue, Time.deltaTime * breatheSpeed * axisMultipliers.z);
        }

        transform.localScale = currentAxisValues;
    }
}
