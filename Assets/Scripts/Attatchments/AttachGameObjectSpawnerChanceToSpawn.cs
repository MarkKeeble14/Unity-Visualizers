using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameObjectCycleSpawner))]
public class AttachGameObjectSpawnerChanceToSpawn : AttachParameter
{
    private GameObjectCycleSpawner spawner;

    private void Awake()
    {
        spawner = GetComponent<GameObjectCycleSpawner>();
    }

    protected override void SetParameter(float value)
    {
        spawner.ChanceToSpawnBatch.Set(value, spawner.ChanceToSpawnBatch.y);
    }
}
