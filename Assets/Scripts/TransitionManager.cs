using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransitionData
{
    public Transition transition;
    public TransitionDirection direction;
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager _Instance { get; private set; }

    [SerializeField] private List<SerializableKeyValuePair<string, Transition>> transitions = new();

    [SerializeField] private TransitionData initialTransition;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;
    }

    private void Start()
    {
        if (initialTransition.transition != null)
        {
            initialTransition.transition.InitiateTransition(initialTransition.direction);
        }
    }

    public void Transition(string transitionKey, TransitionDirection direction)
    {
        foreach (SerializableKeyValuePair<string, Transition> kvp in transitions)
        {
            if (kvp.Key == transitionKey)
            {
                kvp.Value.InitiateTransition(direction);
                break;
            }
        }
    }
}
