using System;
using System.Collections;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    private IEnumerator In(Action onBegin, Action onEnd)
    {
        onBegin?.Invoke();

        isTransitioning = true;

        yield return StartCoroutine(TransitionIn());

        onEnd?.Invoke();

        isTransitioning = false;
    }
    protected abstract IEnumerator TransitionIn();

    private IEnumerator Out(Action onBegin, Action onEnd)
    {
        onBegin?.Invoke();

        isTransitioning = true;

        yield return StartCoroutine(TransitionOut());

        onEnd?.Invoke();

        isTransitioning = false;
    }
    protected abstract IEnumerator TransitionOut();

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

    private bool isTransitioning;
}
