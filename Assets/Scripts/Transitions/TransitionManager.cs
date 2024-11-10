using System;
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

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);
        else _Instance = this;
    }

    public void Transition(string transitionKey, TransitionDirection direction, Action onBegin = null, Action onEnd = null)
    {
        foreach (SerializableKeyValuePair<string, Transition> kvp in transitions)
        {
            if (kvp.Key == transitionKey)
            {
                kvp.Value.InitiateTransition(direction, onBegin, onEnd);
                break;
            }
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
