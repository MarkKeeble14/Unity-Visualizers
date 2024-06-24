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

    [SerializeField] private int beginDestroyingTilesAfterNumTriggers = 2;
    [SerializeField] private int initialSpawnBuffer = 10;
    private int destroyTriggersCount;
    private List<KeyValuePair<GameObject, DestroyLevelTileTrigger>> spawnedTiles = new();

    [Header("Last Tile")]
    [SerializeField] private List<GameEvent> activateOnSpawningLastTile;
    private bool lastTile;
    public bool LastTile { get { return lastTile; } set {  lastTile = value; } }
    private bool spawnedLastTile;

    private void Start()
    {
        for (int i = 0; i < initialSpawnBuffer; i++)
        {
            SpawnTile();
        }
    }

    private void SpawnTile()
    {
        if (spawnedLastTile) return;

        // Spawn tile
        GameObject spawned = Instantiate(levelTile, nextSpawnPos, Quaternion.identity);
        spawned.transform.parent = transform;

        // Attach destroy level tile trigger
        DestroyLevelTileTrigger destroyTrigger = spawned.GetComponentInChildren<DestroyLevelTileTrigger>();
        destroyTrigger.SetTarget(following);
        destroyTrigger.OnTargetEnter += OnEnterDestroyTileTrigger;

        // Track spawned Tile
        spawnedTiles.Add(new(spawned, destroyTrigger));

        // Track info
        nextSpawnPos = nextSpawnPos + nextSpawnDistance;

        if (lastTile)
        {
            spawnedLastTile = true;
            foreach (GameEvent e in activateOnSpawningLastTile)
            {
                e.Activate();
            }
        }
    }

    private void OnEnterDestroyTileTrigger()
    {
        destroyTriggersCount++;
        if (destroyTriggersCount > beginDestroyingTilesAfterNumTriggers)
        {
            KeyValuePair<GameObject, DestroyLevelTileTrigger> toDestroy = spawnedTiles[0];
            spawnedTiles.RemoveAt(0);
            Destroy(toDestroy.Key);

            SpawnTile();
        }
    }

    public void SetLastTile(GameObject newLevelTile)
    {
        levelTile = newLevelTile;
        lastTile = true;
    }
}
