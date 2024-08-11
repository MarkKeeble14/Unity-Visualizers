using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCycleSpawner : MonoBehaviour
{
    [SerializeField] private RelativeOrAbsolute spawnAtType;
    [SerializeField] private GameObject spawningObject;
    [SerializeField] private Vector2 chanceToSpawnBatch;
    public Vector2 ChanceToSpawnBatch { get { return chanceToSpawnBatch; } set { chanceToSpawnBatch = value;} }
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
        StartCoroutine(SpawnLoop(false));
    }

    public void ForceSpawnBatch()
    {
        StartCoroutine(SpawnLoop(true));
    }

    private void Spawn(Vector3 spawnPos)
    {
        // Spawn them with an additional offset
        Instantiate(spawningObject, spawnPos +
            new Vector3(
                RandomHelper.RandomFloat(minMaxXOffset),
                RandomHelper.RandomFloat(minMaxYOffset),
                RandomHelper.RandomFloat(minMaxZOffset)
                ), Quaternion.identity);
    }

    private IEnumerator SpawnLoop(bool forced)
    {
        if (!forced)
        {
            yield return new WaitForSeconds(timeBetweenSpawningBatches);

            if (!RandomHelper.EvaluateChanceTo(chanceToSpawnBatch))
            {
                StartCoroutine(SpawnLoop(false));
                yield break;
            }
        }

        // Find central batch spawn location
        Vector3 spawnPos = new Vector3(
            RandomHelper.RandomFloat(batchMinMaxXSpawn),
            RandomHelper.RandomFloat(batchMinMaxYSpawn),
            RandomHelper.RandomFloat(batchMinMaxZSpawn)
            );

        if (spawnAtType == RelativeOrAbsolute.RELATIVE)
            spawnPos += transform.position;

        // Determine how many belong to batch and thus should be spawned
        for (int i = 0; i < RandomHelper.RandomIntExclusive(minMaxInBatch); i++)
        {
            Spawn(spawnPos);

            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxDelayInBatch));
        }

        // Continue the Loop if not one time
        if (!forced)
            StartCoroutine(SpawnLoop(false));
    }
}
