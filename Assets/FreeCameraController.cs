using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float spedUpMoveSpeed;
    [SerializeField] private float lookSpeed;
    [SerializeField] private Transform positioner;
    private float currentMoveSpeed;

    // Update is called once per frame
    void Update()
    {
        currentMoveSpeed = (Input.GetKey(KeyCode.LeftShift) ? spedUpMoveSpeed : defaultMoveSpeed);

        if (Input.GetKey(KeyCode.W))
        {
            positioner.position += positioner.forward * Time.deltaTime * currentMoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            positioner.position += -positioner.forward * Time.deltaTime * currentMoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            positioner.position += -positioner.right * Time.deltaTime * currentMoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            positioner.position += positioner.right * Time.deltaTime * currentMoveSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            positioner.Rotate(new Vector3(-lookSpeed, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            positioner.Rotate(new Vector3(lookSpeed, 0, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            positioner.Rotate(new Vector3(0, lookSpeed, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            positioner.Rotate(new Vector3(0, -lookSpeed, 0));
        }
    }
}
