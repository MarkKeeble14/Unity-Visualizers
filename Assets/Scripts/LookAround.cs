using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : RecievesInput
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private Vector2 minMaxLook;
    private Vector2 currentLookVector;
    private float returnTimer;
    [SerializeField] private float returnTimerDuration = .25f;

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
        currentLookVector.x = Mathf.Lerp(currentLookVector.x, minMaxLook.x * dir, lookSpeed * Time.deltaTime);
        returnTimer = returnTimerDuration;
    }

    private void LookVertical(float dir)
    {
        currentLookVector.y = Mathf.Lerp(currentLookVector.y, minMaxLook.y * dir, lookSpeed * Time.deltaTime);
        returnTimer = returnTimerDuration;
    }

    private void Update()
    {
        // Add Input
        if (Input.GetKey(KeyCode.A))
            LookHorizontal(-1);
        if (Input.GetKey(KeyCode.D))
            LookHorizontal(1);
        if (Input.GetKey(KeyCode.W))
            LookVertical(1);
        if (Input.GetKey(KeyCode.S))
            LookVertical(-1);

        // Reset if no input is found
        if (returnTimer <= 0)
            currentLookVector = Vector2.Lerp(currentLookVector, Vector2.zero, lookSpeed * Time.deltaTime);
        else
            returnTimer -= Time.deltaTime;

        // Set rotation
        transform.localEulerAngles = new Vector3(-currentLookVector.y, currentLookVector.x, 0);
    }
}
