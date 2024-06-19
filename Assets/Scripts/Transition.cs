using System.Collections;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    private IEnumerator In()
    {
        isTransitioning = true;

        yield return StartCoroutine(TransitionIn());

        isTransitioning = false;
    }
    protected abstract IEnumerator TransitionIn();

    private IEnumerator Out()
    {
        isTransitioning = true;

        yield return StartCoroutine(TransitionOut());

        isTransitioning = false;
    }
    protected abstract IEnumerator TransitionOut();

    public void InitiateTransition(TransitionDirection direction)
    {
        // interrrupt previous transitions
        if (isTransitioning)
            StopAllCoroutines();

        switch (direction)
        {
            case TransitionDirection.IN:
                StartCoroutine(In());
                break;
            case TransitionDirection.OUT:
                StartCoroutine(Out());
                break;
        }
    }

    private bool isTransitioning;
}
