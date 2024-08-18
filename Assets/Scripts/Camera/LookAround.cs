using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : RecievesInput
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private bool clampX;
    [SerializeField] private bool clampY;
    [SerializeField] private Vector2 minMaxLookX;
    [SerializeField] private Vector2 minMaxLookY;
    [SerializeField] private bool enableReturnTimer = true;
    [SerializeField] private float returnTimerDuration = .25f;
    [SerializeField] private Transform subject;
    private Vector2 currentLookVector;
    private float returnTimer;

    public void LookUp()
    {
        LookVertical(1);
    }

    public void LookRight()
    {
        LookHorizontal(1);
    }

    public void LookDown()
    {
        LookVertical(-1);
    }

    public void LookLeft()
    {
        LookHorizontal(-1);
    }

    public override void RecieveInput(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.A:
                LookHorizontal(-1);
                break;
            case KeyCode.D:
                LookHorizontal(1);
                break;
            case KeyCode.W:
                LookVertical(1);
                break;
            case KeyCode.S:
                LookVertical(-1);
                break;
        }
    }

    private void LookHorizontal(float dir)
    {
        currentLookVector.x += lookSpeed * dir * Time.deltaTime;
        returnTimer = returnTimerDuration;
    }

    private void LookVertical(float dir)
    {
        currentLookVector.y += lookSpeed * dir * Time.deltaTime;
        returnTimer = returnTimerDuration;
    }

    private void Update()
    {
        if (enableReturnTimer)
        {
            // Reset if no input is found
            if (returnTimer <= 0)
                currentLookVector = Vector2.Lerp(currentLookVector, Vector2.zero, lookSpeed * Time.deltaTime);
            else
                returnTimer -= Time.deltaTime;
        }

        // Set rotation
        if (clampX)
        {
            currentLookVector.x = Mathf.Clamp(currentLookVector.x, minMaxLookX.x, minMaxLookX.y);
        }
        if (clampY)
        {
            currentLookVector.y = Mathf.Clamp(currentLookVector.y, minMaxLookY.x, minMaxLookY.y);
        }
        subject.localEulerAngles = new Vector3(-currentLookVector.y, currentLookVector.x, 0);
    }
}
