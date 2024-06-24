using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeScale : MonoBehaviour
{
    [SerializeField, Tooltip("Randomizes based on minMaxScaleX")] bool matchScaleOnAllAxis;

    [SerializeField] private Vector2 minMaxScaleX = new Vector2(1, 2);
    [SerializeField] private Vector2 minMaxScaleY = new Vector2(1, 2);
    [SerializeField] private Vector2 minMaxScaleZ = new Vector2(1, 2);

    private void Awake()
    {
        if (matchScaleOnAllAxis)
        {
            transform.localScale = Vector3.one * RandomHelper.RandomFloat(minMaxScaleX);
        } else
        {
            transform.localScale =
                new Vector3(
                        RandomHelper.RandomFloat(minMaxScaleX),
                        RandomHelper.RandomFloat(minMaxScaleY),
                        RandomHelper.RandomFloat(minMaxScaleZ)
                );
        }
    }
}
