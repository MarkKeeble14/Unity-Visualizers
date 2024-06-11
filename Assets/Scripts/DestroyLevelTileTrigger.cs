using System;
using UnityEngine;

public class DestroyLevelTileTrigger : MonoBehaviour
{
    private Transform target;

    public Action OnTargetEnter;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            OnTargetEnter!.Invoke();
            Destroy(gameObject);
        }
    }
}
