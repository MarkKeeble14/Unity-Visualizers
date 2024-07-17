using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionedCamera : MonoBehaviour
{
    [SerializeField] private Transform positioner;
    public Transform Positioner => positioner;
    [SerializeField] private Camera camera;
    public Camera Camera => camera;
}
