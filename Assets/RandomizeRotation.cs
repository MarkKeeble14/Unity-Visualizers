using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxRotX;
    [SerializeField] private Vector2 minMaxRotY;
    [SerializeField] private Vector2 minMaxRotZ;

    private void Awake()
    {
        transform.localEulerAngles = 
            new Vector3(
                RandomHelper.RandomFloat(minMaxRotX),
                RandomHelper.RandomFloat(minMaxRotY),
                RandomHelper.RandomFloat(minMaxRotZ)
            );
    }
}
