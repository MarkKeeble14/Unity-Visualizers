using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromInitialPosition : MonoBehaviour
{
    [SerializeField] private MathHelper.AlterationMethod methodOfChange; 
    [SerializeField] private Vector3 distanceFromInitialPosition;
    [SerializeField] private float moveSpeed;
    private Vector3 goalPosition;

    private void Awake()
    {
        // Find Goal Position
        goalPosition = transform.position + distanceFromInitialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == goalPosition)
            return;
        // Change Value
        transform.position = MathHelper.GetNextValue(transform.position, goalPosition, moveSpeed, methodOfChange, true);
    }
}
