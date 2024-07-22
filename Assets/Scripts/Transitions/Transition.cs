using System;
using System.Collections;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    [Header("Base Transition Settings")]
    [SerializeField] protected float inSpeed = 1;
    [SerializeField] protected float outSpeed = 1;
    private bool isTransitioning;

    private IEnumerator In(Action onBegin, Action onEnd)
    {
        Debug.Log("Beginning Transition In: " + gameObject.name);

        onBegin?.Invoke();

        isTransitioning = true;

        yield return StartCoroutine(TransitionIn(inSpeed));

        onEnd?.Invoke();

        isTransitioning = false;
    }
    protected abstract IEnumerator TransitionIn(float speed);

    private IEnumerator Out(Action onBegin, Action onEnd)
    {
        Debug.Log("Beginning Transition Out: " + gameObject.name);

        onBegin?.Invoke();

        isTransitioning = true;

        yield return StartCoroutine(TransitionOut(outSpeed));

        onEnd?.Invoke();

        isTransitioning = false;
    }

    protected abstract IEnumerator TransitionOut(float speed);

    public void InitiateTransition(TransitionDirection direction, Action onBegin = null, Action onEnd = null)
    {
        // interrrupt previous transitions
        if (isTransitioning)
            StopAllCoroutines();

        switch (direction)
        {
            case TransitionDirection.IN:
                StartCoroutine(In(onBegin, onEnd));
                break;
            case TransitionDirection.OUT:
                StartCoroutine(Out(onBegin, onEnd));
                break;
        }
    }
}
