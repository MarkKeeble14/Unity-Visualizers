using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMovement : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Vector3 centerPos;
    [SerializeField] private float speed;
    [SerializeField] private bool autoMove;
    [SerializeField] private float startAtPercentAroundCircle;
    private Vector3 point;
    private float percentAroundCircle;

    private void Awake()
    {
        percentAroundCircle = startAtPercentAroundCircle;
    }

    public void SetRadiusPosition(float v)
    {
        percentAroundCircle = startAtPercentAroundCircle + v;
    }

    private void Update()
    {
        if (autoMove)
        {
            percentAroundCircle += Time.deltaTime * speed;
        }

        point.x = centerPos.x + Mathf.Sin(percentAroundCircle * Mathf.PI * 2) * radius;
        point.y = centerPos.y + Mathf.Cos(percentAroundCircle * Mathf.PI * 2) * radius;
        point.z = centerPos.z;
        transform.position = point;
    }
}
