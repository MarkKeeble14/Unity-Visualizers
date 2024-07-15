using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float spedUpMoveSpeed;
    [SerializeField] private float lookSpeed;
    private float currentMoveSpeed;
    [SerializeField] private KeyCode speedUpButton = KeyCode.LeftShift;

    [Header("References")]
    [SerializeField] private Transform positioner;
    [SerializeField] private Camera freeCam;
    private GameObject prevCamera;

    public static FreeCameraController _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    public void Activate()
    {
        prevCamera = Camera.main.gameObject;
        prevCamera.gameObject.SetActive(false);

        freeCam.transform.position = prevCamera.transform.position;
        freeCam.transform.rotation = prevCamera.transform.rotation;

        freeCam.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        freeCam.gameObject.SetActive(false);

        prevCamera.transform.position = freeCam.transform.position;
        prevCamera.transform.rotation = freeCam.transform.rotation;

        prevCamera.gameObject.SetActive(true);
        prevCamera = null;
    }

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
        currentMoveSpeed = (Input.GetKey(speedUpButton) ? spedUpMoveSpeed : defaultMoveSpeed);
    }
}
