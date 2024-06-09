using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{

    [SerializeField] private GameObject spawningObject;
    [SerializeField] private Vector2 chanceToSpawnBatch;
    [SerializeField] private Vector2 minMaxInBatch;
    [SerializeField] private Vector2 batchMinMaxXSpawn;
    [SerializeField] private Vector2 batchMinMaxYSpawn;
    [SerializeField] private Vector2 batchMinMaxZSpawn;
    [SerializeField] private Vector2 minMaxXOffset;
    [SerializeField] private Vector2 minMaxYOffset;
    [SerializeField] private Vector2 minMaxZOffset;
    [SerializeField] private float timeBetweenSpawningBatches;
    [SerializeField] private Vector2 minMaxDelayInBatch;

    private void Awake()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // Wait
        yield return new WaitForSeconds(timeBetweenSpawningBatches);

        // Determine if batch should spawn
        if (RandomHelper.EvaluateChanceTo(chanceToSpawnBatch))
        {
            // Find central batch spawn location
            Vector3 spawnPos = new Vector3(
                RandomHelper.RandomFloat(batchMinMaxXSpawn),
                RandomHelper.RandomFloat(batchMinMaxYSpawn),
                RandomHelper.RandomFloat(batchMinMaxZSpawn)
                );

            // Determine how many belong to batch and thus should be spawned
            for (int i = 0; i < RandomHelper.RandomIntExclusive(minMaxInBatch); i++)
            {
                // Spawn them with an additional offset
                Instantiate(spawningObject, spawnPos +
                    new Vector3(
                        RandomHelper.RandomFloat(minMaxXOffset),
                        RandomHelper.RandomFloat(minMaxYOffset),
                        RandomHelper.RandomFloat(minMaxZOffset)
                        ), Quaternion.identity);

                yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxDelayInBatch));
            }

        }

        // Continue the Loop
        StartCoroutine(SpawnLoop());
    }
}
