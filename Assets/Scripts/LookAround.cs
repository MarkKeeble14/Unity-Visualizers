using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : MonoBehaviour
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private Vector2 minMaxLook;
    private Vector2 currentLookVector;
    private bool hasInput;

    private void Update()
    {
        hasInput = false;

        // Add Input
        if (Input.GetKey(KeyCode.A))
        {
            currentLookVector.x = Mathf.Lerp(currentLookVector.x, -minMaxLook.x, lookSpeed * Time.deltaTime);
            hasInput = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentLookVector.x = Mathf.Lerp(currentLookVector.x, minMaxLook.x, lookSpeed * Time.deltaTime);
            hasInput = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            currentLookVector.y = Mathf.Lerp(currentLookVector.y, minMaxLook.y, lookSpeed * Time.deltaTime);
            hasInput = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentLookVector.y = Mathf.Lerp(currentLookVector.y, -minMaxLook.y, lookSpeed * Time.deltaTime);
            hasInput = true;
        }

        // Reset if no input is found
        if (!hasInput)
            currentLookVector = Vector2.Lerp(currentLookVector, Vector2.zero, lookSpeed * Time.deltaTime);

        // Set rotation
        transform.localEulerAngles = new Vector3(-currentLookVector.y, currentLookVector.x, 0);
    }
}
