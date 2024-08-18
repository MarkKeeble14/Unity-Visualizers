using TMPro;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float spedUpMoveSpeed;
    [SerializeField] private float freeLookSensitivity = 3f;
    [SerializeField] private float zoomSensitivity = 10f;
    [SerializeField] private float fastZoomSensitivity = 50f;
    [SerializeField] private KeyCode speedUpButton = KeyCode.LeftShift;

    [Header("References")]
    [SerializeField] private PositionedCamera freeCam;
    private PositionedCamera prevCamera;

    private float currentMoveSpeed;
    public bool Active { get; private set; }
    public bool Pause { get; set; }

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
        if (QuitUtil.isQuitting) return;
        Deactivate();
    }

    public void Activate()
    {
        Active = true;
        Pause = false;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        prevCamera = Camera.main.GetComponent<PositionedCamera>();

        freeCam.Positioner.position = prevCamera.Positioner.position;
        freeCam.Camera.transform.position = prevCamera.Camera.transform.position;
        freeCam.Positioner.rotation = prevCamera.Positioner.rotation;
        freeCam.Camera.transform.rotation = prevCamera.Camera.transform.rotation;

        prevCamera.Camera.gameObject.SetActive(false);
        freeCam.Camera.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        Active = false;
        Pause = false;
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

        prevCamera.Positioner.position = freeCam.Positioner.position;
        prevCamera.Camera.transform.position = freeCam.Camera.transform.position;
        prevCamera.Positioner.rotation = freeCam.Positioner.rotation;
        prevCamera.Camera.transform.rotation = freeCam.Camera.transform.rotation;

        freeCam.Camera.gameObject.SetActive(false);
        prevCamera.Camera.gameObject.SetActive(true);
        prevCamera = null;
    }

    public void MoveForward()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (freeCam.Positioner.forward * currentMoveSpeed * Time.deltaTime);
    }

    public void MoveRight()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (freeCam.Positioner.right * currentMoveSpeed * Time.deltaTime);
    }

    public void MoveBack()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (-freeCam.Positioner.forward * currentMoveSpeed * Time.deltaTime);
    }

    public void MoveLeft()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (-freeCam.Positioner.right * currentMoveSpeed * Time.deltaTime);
    }

    public void RotateUp()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (Vector3.up * currentMoveSpeed * Time.deltaTime);
    }

    public void RotateRight()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (-freeCam.Positioner.up * currentMoveSpeed * Time.deltaTime);
    }

    public void RotateDown()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (-Vector3.up * currentMoveSpeed * Time.deltaTime);
    }

    public void RotateLeft()
    {
        if (!Active) return;
        if (Pause) return;

        freeCam.Positioner.position = freeCam.Positioner.position + (freeCam.Positioner.up * currentMoveSpeed * Time.deltaTime);
    }

    void Update()
    {
        if (!Active) return;

        if (Input.GetMouseButtonDown(1))
        {
            VisualizerManager._Instance.SelectControlScheme(ControlScheme.VISUALIZER);
        }

        if (Pause) return;

        bool fastMode = Input.GetKey(speedUpButton);
        currentMoveSpeed = (fastMode ? spedUpMoveSpeed : defaultMoveSpeed);

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        float newRotationX = freeCam.Positioner.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
        float newRotationY = freeCam.Positioner.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
        freeCam.Positioner.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
    }
}
