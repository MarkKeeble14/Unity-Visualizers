using UnityEngine;

public class AttachForceSpawnGameObjects : AttachParameter
{
    [Header("References")]
    [SerializeField] private GameObjectSpawner spawner;

    [Header("Cooldown Settings")]
    [SerializeField] private bool useCooldown;
    [SerializeField] private float cooldown;
    [SerializeField] private float cooldownRate = 1;
    private float currentCooldown;

    private void Update()
    {
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime * cooldownRate;
    }

    protected override void SetParameter(float value)
    {
        if (!useCooldown)
        {
            spawner.ForceSpawnBatch();
        }
        else if (currentCooldown <= 0)
        {
            spawner.ForceSpawnBatch();
            currentCooldown = cooldown;
        }
    }
}