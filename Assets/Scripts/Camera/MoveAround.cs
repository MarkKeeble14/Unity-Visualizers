using UnityEngine;

public class MoveAround : RecievesInput
{
    [SerializeField] private Transform subject;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool alignOnLayer;
    [SerializeField] private LayerMask walkOnLayer;
    [SerializeField] private float height;
    Vector3 groundAlignedPosition;
    RaycastHit hit;

    public void MoveForward()
    {
        subject.position += Camera.main.transform.forward * moveSpeed * Time.deltaTime;
    }

    public void MoveRight()
    {
        subject.position += Camera.main.transform.right * moveSpeed * Time.deltaTime;
    }

    public void MoveBackward()
    {
        subject.position -= Camera.main.transform.forward * moveSpeed * Time.deltaTime;
    }

    public void MoveLeft()
    {
        subject.position -= Camera.main.transform.right * moveSpeed * Time.deltaTime;
    }

    private void Update()
    {
        if (alignOnLayer) { AlignOnLayer(); }
    }

    private void AlignOnLayer()
    {
        if (Physics.Raycast(subject.position, Vector3.down, out hit, Mathf.Infinity, walkOnLayer))
        {
            groundAlignedPosition = subject.position;
            groundAlignedPosition.y = hit.point.y + height;
            subject.position = groundAlignedPosition;
        }
    }

    public override void RecieveInput(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.A:
                MoveLeft();
                break;
            case KeyCode.D:
                MoveRight();
                break;
            case KeyCode.W:
                MoveForward();
                break;
            case KeyCode.S:
                MoveBackward();
                break;
        }
    }
}
