using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOnGround : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxOffsetX = new Vector2(-.5f, .5f);
    [SerializeField] private Vector2 minMaxOffsetY = new Vector2(-.5f, .5f);
    [SerializeField] private Vector2 minMaxOffsetZ = new Vector2(-.5f, .5f);
    [SerializeField] private LayerMask spawnOnlayer;

    void Start()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up * 999, Vector3.down, out hit, Mathf.Infinity, spawnOnlayer);
        transform.position = hit.point + new Vector3(
                RandomHelper.RandomFloat(minMaxOffsetX),
                RandomHelper.RandomFloat(minMaxOffsetY) + GetGameObjectBounds(gameObject).extents.y,
                RandomHelper.RandomFloat(minMaxOffsetZ)
            );
    }

    private Bounds GetGameObjectBounds(GameObject obj)
    {
        return obj.GetComponentInChildren<Renderer>().bounds;
    }
}
