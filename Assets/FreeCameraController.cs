using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float spedUpMoveSpeed;
    [SerializeField] private float lookSpeed;
    [SerializeField] private Transform positioner;
    private float currentMoveSpeed;

    public void MoveForward()
    {
        positioner.position += positioner.forward * Time.deltaTime * currentMoveSpeed;
    }

    public void MoveRight()
    {
        positioner.position += positioner.right * Time.deltaTime * currentMoveSpeed;
    }

    public void MoveBack()
    {
        positioner.position += -positioner.forward * Time.deltaTime * currentMoveSpeed;
    }

    public void MoveLeft()
    {
        positioner.position += -positioner.right * Time.deltaTime * currentMoveSpeed;
    }

    public void RotateUp()
    {
        positioner.Rotate(new Vector3(-lookSpeed, 0, 0));
    }

    public void RotateRight()
    {
        positioner.Rotate(new Vector3(0, lookSpeed, 0));
    }

    public void RotateDown()
    {
        positioner.Rotate(new Vector3(lookSpeed, 0, 0));
    }

    public void RotateLeft()
    {
        positioner.Rotate(new Vector3(0, -lookSpeed, 0));
    }

    // Update is called once per frame
    void Update()
    {
        currentMoveSpeed = (Input.GetKey(KeyCode.LeftShift) ? spedUpMoveSpeed : defaultMoveSpeed);
    }
}
