using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AutomationInput
{
    [SerializeField] private PercentageMap<List<KeyCode>> keys;
    public PercentageMap<List<KeyCode>> Keys { get { return keys; } }
    [SerializeField] private Vector2 minMaxDuration;
    public Vector2 MinMaxDuration { get { return minMaxDuration; } }
}

[System.Serializable]
public struct AutomationInputSequence
{
    [SerializeField] private List<AutomationInput> input;
    public List<AutomationInput> Input { get { return input; } }
}

public class AutomateInputSequences : MonoBehaviour
{
    [SerializeField] private RecievesInput automating;
    [SerializeField] private PercentageMap<AutomationInputSequence> availableSequences;

    [SerializeField] private Vector2 minMaxTimeBetweenActivations;
    [SerializeField] private Vector2 chanceToActivate;

    private void Start()
    {
        StartCoroutine(LogicLoop());
    }

    private IEnumerator PlayOutInputSequence(AutomationInputSequence sequence)
    {
        float t;
        List<KeyCode> keys;
        foreach (AutomationInput automationInput in sequence.Input)
        {
            t = 0;
            keys = automationInput.Keys.GetOption();

            while (t < RandomHelper.RandomFloat(automationInput.MinMaxDuration))
            {
                t += Time.deltaTime;

                foreach (KeyCode key in keys)
                    automating.RecieveInput(key);
                
                yield return null;
            }
        }
    }

    private IEnumerator LogicLoop()
    {
        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenActivations));

        if (RandomHelper.EvaluateChanceTo(chanceToActivate))
        {
            yield return StartCoroutine(PlayOutInputSequence(availableSequences.GetOption()));
        }

        StartCoroutine(LogicLoop());
    }
}

