using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelTileSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject levelTile;
    [SerializeField] private Transform following;

    [Header("Settings")]
    [SerializeField] private float spawnAfterDistance;
    [SerializeField] private Vector3 nextSpawnPos;
    [SerializeField] private Vector3 nextSpawnDistance;
    private float distanceSinceLastSpawn;
    private Vector3 lastSpawnedAt;

    [SerializeField] private float beginDestroyingAfterDistance;
    private float totalDistanceTravelled;
    private List<GameObject> spawnedTiles = new List<GameObject>();

    [SerializeField] private int initialSpawnBuffer = 10;

    private void Start()
    {
        for (int i = 0; i < initialSpawnBuffer; i++)
        {
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceSinceLastSpawn = Vector3.Distance(following.position, lastSpawnedAt);
        if (distanceSinceLastSpawn >= spawnAfterDistance)
        {
            SpawnTile();
        }
    }

    private void SpawnTile()
    {
        // Spawn tile
        GameObject spawned = Instantiate(levelTile, nextSpawnPos, Quaternion.identity);
        spawnedTiles.Add(spawned);
        spawned.transform.parent = transform;

        // Track info
        totalDistanceTravelled += distanceSinceLastSpawn;
        nextSpawnPos = nextSpawnPos + nextSpawnDistance;
        lastSpawnedAt = following.position;

        // Check if need to destroy
        if (totalDistanceTravelled >= beginDestroyingAfterDistance)
        {
            GameObject toDestroy = spawnedTiles[0];
            spawnedTiles.RemoveAt(0);
            Destroy(toDestroy);
        }
    }
}
