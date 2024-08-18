using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    [SerializeField] private RelativeOrAbsolute spawnAtType;
    [SerializeField] private Vector2 minMaxToSpawn;
    [SerializeField] private Vector2 minMaxXOffset;
    [SerializeField] private Vector2 minMaxYOffset;
    [SerializeField] private Vector2 minMaxZOffset;
    [SerializeField] private PercentageMap<GameObject> spawnChoices = new();
    [SerializeField] private bool clearPreviouslySpawned;
    [SerializeField] private List<GameObject> removeBeforeSpawning = new();
    [SerializeField] private bool spawnOnAwake;
    private GameObject cachedObj;
    [SerializeField] private Transform parentOfSpawned;

    private void Awake()
    {
        if (spawnOnAwake) Spawn();
    }

    public void Spawn()
    {
        // Destroy previous
        if (clearPreviouslySpawned)
        {
            while (removeBeforeSpawning.Count > 0)
            {
                cachedObj = removeBeforeSpawning[0];
                removeBeforeSpawning.RemoveAt(0);
                Destroy(cachedObj);
            }
        }

        // Spawn new
        for (int i = 0; i < RandomHelper.RandomIntExclusive(minMaxToSpawn); ++i)
        {
            Vector3 spawnPos = new Vector3(
                    RandomHelper.RandomFloat(minMaxXOffset),
                    RandomHelper.RandomFloat(minMaxYOffset),
                    RandomHelper.RandomFloat(minMaxZOffset)
                    );

            if (spawnAtType == RelativeOrAbsolute.RELATIVE)
                spawnPos += transform.position;

            // Spawn them with an additional offset
            cachedObj = Instantiate(spawnChoices.GetOption(), parentOfSpawned);
            cachedObj.transform.position = spawnPos;

            if (clearPreviouslySpawned)
            {
                removeBeforeSpawning.Add(cachedObj);
            }
        }
    }
}
