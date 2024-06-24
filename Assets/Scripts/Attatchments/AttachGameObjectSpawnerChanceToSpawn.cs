using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameObjectSpawner))]
public class AttachGameObjectSpawnerChanceToSpawn : AttachParameter
{
    private GameObjectSpawner spawner;

    private void Awake()
    {
        spawner = GetComponent<GameObjectSpawner>();
    }

    protected override void SetParameter(float value)
    {
        spawner.ChanceToSpawnBatch.Set(value, spawner.ChanceToSpawnBatch.y);
    }
}
