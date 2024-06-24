using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public enum AnimationDirection
{
    FORWARD,
    BACKWARD,
    RANDOM
}

public class IdleRotation : MonoBehaviour
{
    [SerializeField] private Axis rotateOn;

    [SerializeField] private AnimationDirection startDirection;
    [SerializeField] private float minValue = -5f;
    [SerializeField] private float maxValue = 5f;
    private float currentValue;
    private float targetValue;

    [SerializeField] private float rotateSpeed;

    private void Start()
    {
        switch (startDirection)
        {
            case AnimationDirection.FORWARD:
                targetValue = maxValue;
                break;
            case AnimationDirection.BACKWARD:
                targetValue = minValue;
                break;
            case AnimationDirection.RANDOM:
                if (RandomHelper.RandomBool())
                    targetValue = maxValue;
                else
                    targetValue = minValue;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Logic to find out current value
        currentValue = Mathf.MoveTowards(currentValue, targetValue, Time.deltaTime * rotateSpeed);
        if (currentValue <= minValue)
        {
            targetValue = maxValue;
        } else if (currentValue >= maxValue)
        {
            targetValue = minValue;
        }

        // Setting Value
        switch (rotateOn)
        {
            case Axis.X:
                transform.localEulerAngles = new Vector3(currentValue, 0, 0);
                break;
            case Axis.Y:
                transform.localEulerAngles = new Vector3(0, currentValue, 0);
                break;
            case Axis.Z:
                transform.localEulerAngles = new Vector3(0, 0, currentValue);
                break;
        }
    }
}
