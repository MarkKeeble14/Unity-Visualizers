using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInCanvasGroup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float goal = 0;
    [SerializeField] private float delay = 3;
    [SerializeField] private float rateOfChange;
    [SerializeField] private MathHelper.AlterationMethod method;

    [Header("References")]
    [SerializeField] private CanvasGroup cv;

    private void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(delay);

        while (cv.alpha != goal) 
        {
            cv.alpha = MathHelper.GetNextValue(cv.alpha, goal, rateOfChange, method, true);

            yield return null;
        }
    }
}
