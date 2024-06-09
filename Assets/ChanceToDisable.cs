using System.Collections;
using UnityEngine;


public class ChanceToDisable : MonoBehaviour
{
    [SerializeField] private Vector2 chanceTo;

    private void Awake()
    {
        if (RandomHelper.EvaluateChanceTo(chanceTo))
            gameObject.SetActive(false);
    }
}
