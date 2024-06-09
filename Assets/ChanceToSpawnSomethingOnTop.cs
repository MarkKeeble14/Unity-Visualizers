using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToSpawnSomethingOnTop : MonoBehaviour
{
    [SerializeField] private Vector2 chanceToSpawnSomething;
    [SerializeField] private PercentageMap<GameObject> optionsToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        if (RandomHelper.EvaluateChanceTo(chanceToSpawnSomething))
            Instantiate(optionsToSpawn.GetOption(), transform);
    }
}
